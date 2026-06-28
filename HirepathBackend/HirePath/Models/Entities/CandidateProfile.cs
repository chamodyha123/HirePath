using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class CandidateProfile : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? Location { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public int YearsOfExperience { get; set; }

        public ICollection<CandidateSkill> Skills { get; set; } = new List<CandidateSkill>();
        public ICollection<CandidateEducation> Educations { get; set; } = new List<CandidateEducation>();
        public ICollection<CandidateExperience> Experiences { get; set; } = new List<CandidateExperience>();
        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}