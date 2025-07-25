using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.Auth
{
    public class ForgetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
    }
} 