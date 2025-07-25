using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using ServiceAbstraction;
using Shared.Dtos.Auth;
using Service.Helpers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using Shared.Dtos;

namespace DoctorEase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminAuthController> _logger;
        private readonly IMailingService _mailingService;

        public AdminAuthController(
            IServiceManager serviceManager,
            IConfiguration configuration,
            ILogger<AdminAuthController> logger,
            IMailingService mailingService)
        {
            _serviceManager = serviceManager;
            _configuration = configuration;
            _logger = logger;
            _mailingService = mailingService;
        }

        #region Admin Login

       
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AdminLoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid login data." });

            try
            {
                _logger.LogInformation("🔍 Login attempt for email: {Email}", loginDto.Email);

                var jwtSecret = _configuration["JWT:Secret"] ?? "your-super-secret-key-with-at-least-32-characters";
                var adminResult = await _serviceManager.AuthService.LoginAsync(loginDto, jwtSecret);

                _logger.LogInformation("✅ Login successful for email: {Email}", loginDto.Email);

                return Ok(adminResult);
            }
            catch
            {
                _logger.LogWarning("❌ Login failed for email: {Email}", loginDto.Email);
                return Unauthorized(new { message = "Invalid email or password." });
            }
        }

        #endregion

        #region Change Password

        
        [Authorize(Roles = "Admin")]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data." });

            var result = await _serviceManager.AuthService.ChangePasswordAsync(dto);
            if (!result)
                return Unauthorized(new { message = "Invalid email or old password." });

            return Ok(new { message = "Password changed successfully." });
        }

        #endregion

        #region Forget Password
        [AllowAnonymous]
        [HttpPost("forget-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid email address." });

            try
            {
                _logger.LogInformation("🔍 Forget password request for email: {Email}", dto.Email);

                // تحقق فعلي من وجود الإيميل في النظام
                var emailExists = await _serviceManager.AuthService.CheckEmailExistsAsync(dto.Email);

                if (!emailExists)
                {
                    _logger.LogWarning("❌ Forget password - email not registered: {Email}", dto.Email);
                    return NotFound(new { message = "❌ This email is not registered in the system. Please check your email and try again." });
                }

                // توليد التوكن
                var resetToken = await _serviceManager.AuthService.GenerateResetPasswordTokenAsync(dto.Email);

                // بناء الرابط
                var fullResetUrl = $"https://yourfrontenddomain.com/reset-password?email={Uri.EscapeDataString(dto.Email)}&token={Uri.EscapeDataString(resetToken)}";

                // تجهيز الإيميل
                var email = new Email
                {
                    To = dto.Email,
                    Subject = "🔑 Reset Password Request - DoctorEase",
                    Body = $@"
Hello,

You have requested to reset your password for DoctorEase.

Please click the link below to reset your password:
{fullResetUrl}

If you did not request this password reset, please ignore this email.

Best regards,
DoctorEase Team"
                };

                // إرسال الإيميل
                _mailingService.SendEmail(email);

                _logger.LogInformation("✅ Reset password email sent to: {Email}", dto.Email);

                return Ok(new { message = "✅ Reset password link has been sent to your email successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing forget password for email: {Email}", dto.Email);
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        #endregion

        #region Reset Password
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data." });

            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match." });

            var result = await _serviceManager.AuthService.ResetPasswordAsync(dto);

            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            return Ok(new { message = "Password has been reset successfully." });
        }


        #endregion
    }
}
