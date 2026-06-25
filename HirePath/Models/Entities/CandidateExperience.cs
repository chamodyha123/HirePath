using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class CandidateExperience : BaseEntity
    {
        public int CandidateProfileId { get; set; }
        public CandidateProfile? CandidateProfile { get; set; }

        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
    }
}