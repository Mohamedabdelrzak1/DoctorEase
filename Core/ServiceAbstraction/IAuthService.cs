using Domain.Models;
using Shared.Dtos.Auth;
using Microsoft.AspNetCore.Identity;

namespace ServiceAbstraction
{
    public interface IAuthService
    {
        Task<AdminResultDto> LoginAsync(AdminLoginDto loginDto, string jwtSecret);

        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);

        Task<bool> CheckEmailExistsAsync(string email);

        Task<string?> GenerateResetPasswordTokenAsync(string email);

        
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
