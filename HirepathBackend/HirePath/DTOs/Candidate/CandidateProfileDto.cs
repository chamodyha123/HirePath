using System;
using System.Collections.Generic;

namespace HirePathAI.API.DTOs.Candidate
{
    public class CandidateProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsProfileComplete { get; set; }

        public List<CandidateSkillDto> Skills { get; set; } = new();
        public List<CandidateEducationDto> Educations { get; set; } = new();
        public List<CandidateExperienceDto> Experiences { get; set; } = new();
        public List<ResumeDto> Resumes { get; set; } = new();
    }

    public class CreateCandidateProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public int YearsOfExperience { get; set; }
    }

    public class UpdateCandidateProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public int YearsOfExperience { get; set; }
    }
}