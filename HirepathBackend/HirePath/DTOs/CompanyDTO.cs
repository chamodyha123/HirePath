using HirePathAI.API.Models.Enums;

namespace HirePathAI.DTOs
{
    public class CompanyCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public string? LogoUrl { get; set; }
        public CompanyStatus Status { get; set; } = CompanyStatus.Pending;
    }

    public class CompanyResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public string? LogoUrl { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
