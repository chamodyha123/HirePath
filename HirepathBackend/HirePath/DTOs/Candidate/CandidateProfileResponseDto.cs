using HirePathAI.API.Models.Enums;  

namespace HirePathAI.API.DTOs.Candidate  
{
    public class CandidateProfileResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? Location { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public int YearsOfExperience { get; set; }

        public List<SkillDto> Skills { get; set; } = new();
        public List<EducationDto> Educations { get; set; } = new();
        public List<ExperienceDto> Experiences { get; set; } = new();
        public ResumeDto? Resume { get; set; }
    }

    public class SkillDto
    {
        public int Id { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public SkillLevel Level { get; set; }
    }

    public class EducationDto
    {
        public int Id { get; set; }
        public string Institute { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class ExperienceDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class ResumeDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public bool IsPrimary { get; set; }
    }
}