using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Auth
{
    public class ResendOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}