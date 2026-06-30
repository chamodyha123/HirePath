using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Candidate  // 
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public required string FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "Headline cannot exceed 100 characters")]
        public string? Headline { get; set; }

        [StringLength(500, ErrorMessage = "Summary cannot exceed 500 characters")]
        public string? Summary { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string? Location { get; set; }

        [Url(ErrorMessage = "Invalid LinkedIn URL format")]
        public string? LinkedInUrl { get; set; }

        [Url(ErrorMessage = "Invalid Portfolio URL format")]
        public string? PortfolioUrl { get; set; }
    }
}