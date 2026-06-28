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

        // ✅ Enum is correct and default is good
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

        public string? CoverLetter { get; set; }

        // ⚠️ FIX: prevent EF precision warning (important)
        public decimal? MatchScore { get; set; }

        public string? RecruiterNotes { get; set; }

        // ✅ Better initialization (cleaner than new List<Interview>())
        public ICollection<Interview> Interviews { get; set; } = new HashSet<Interview>();

        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public string? CompanyFeedback { get; set; }
    }
}