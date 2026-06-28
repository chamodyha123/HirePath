using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Auth
{
    public class ResendOtpDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}