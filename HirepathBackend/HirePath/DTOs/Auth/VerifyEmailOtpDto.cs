using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Auth
{
    public class VerifyEmailOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Otp { get; set; } = string.Empty;
    }
}