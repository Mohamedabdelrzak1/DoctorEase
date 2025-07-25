using Domain.Contracts;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceAbstraction;
using Shared.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Service.Service
{
    public class AuthService(IUnitOfWork _unitOfWork, IOptions<JwtOptions> _jwtOptions) : IAuthService
    {
        private static readonly Dictionary<string, string> _resetTokens = new(); // للتجارب فقط - استبدله بـ DB أو Redis في الإنتاج

        public async Task<AdminResultDto> LoginAsync(AdminLoginDto loginDto, string jwtSecret)
        {
            var admins = await _unitOfWork.GetRepository<Admin, int>().GetAllAsync();
            var admin = admins.FirstOrDefault(a => a.Email.Equals(loginDto.Email, StringComparison.OrdinalIgnoreCase));

            if (admin == null || admin.PasswordHash != HashPassword(loginDto.Password))
                throw new UnAuthorizedException("Invalid email or password.");

            var token = GenerateJwtToken(admin);

            return new AdminResultDto
            {
                Email = admin.Email,
                DisplayName = "Admin",
                Token = token
            };
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var admins = await _unitOfWork.GetRepository<Admin, int>().GetAllAsync();
            var admin = admins.FirstOrDefault(a => a.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));

            if (admin == null || admin.PasswordHash != HashPassword(dto.OldPassword))
                return false;

            admin.PasswordHash = HashPassword(dto.NewPassword);
            _unitOfWork.GetRepository<Admin, int>().Update(admin);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<string?> GenerateResetPasswordTokenAsync(string email)
        {
            var admins = await _unitOfWork.GetRepository<Admin, int>().GetAllAsync();
            var admin = admins.FirstOrDefault(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (admin == null)
                return null;

            var resetToken = Guid.NewGuid().ToString();

            // ❌ غير آمن للإنتاج - لأغراض الاختبار فقط
            _resetTokens[email] = resetToken;

            return resetToken;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var admins = await _unitOfWork.GetRepository<Admin, int>().GetAllAsync();
            var admin = admins.FirstOrDefault(a => a.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));

            if (admin == null)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid email." });

            if (!_resetTokens.TryGetValue(dto.Email, out var storedToken) || storedToken != dto.Token)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid or expired reset token." });
            }

            admin.PasswordHash = HashPassword(dto.NewPassword);
            _unitOfWork.GetRepository<Admin, int>().Update(admin);
            await _unitOfWork.SaveChangesAsync();

            // إزالة التوكن بعد الاستخدام
            _resetTokens.Remove(dto.Email);

            return IdentityResult.Success;
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            
            var adminRepo = _unitOfWork.GetRepository<Admin, int>();
            var admins = await adminRepo.GetAllAsync();
            return admins.Any(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            
        }


        #region Helpers

        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private string GenerateJwtToken(Admin admin)
        {
            var jwtOptions = _jwtOptions.Value;

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Email),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(jwtOptions.DurationInMinutes),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        

        #endregion
    }
}
