using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class CandidateEducation : BaseEntity
    {
        public int CandidateProfileId { get; set; }
        public CandidateProfile? CandidateProfile { get; set; }

        public string Institute { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}