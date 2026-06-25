using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class JobApplication : BaseEntity
    {
        public int JobId { get; set; }
        public Job? Job { get; set; }

        public int CandidateProfileId { get; set; }
        public CandidateProfile? CandidateProfile { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
        public string? CoverLetter { get; set; }
        public decimal? MatchScore { get; set; }
        public string? RecruiterNotes { get; set; }

        public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    }
}