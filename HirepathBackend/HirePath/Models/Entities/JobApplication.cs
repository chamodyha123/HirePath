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

        // The resume actually submitted for this application — resolved at
        // apply-time (either the one the candidate picked, or their primary
        // resume) and then frozen here so later resume edits/uploads don't
        // change what a recruiter is looking at for this application.
        public int? ResumeId { get; set; }
        public Resume? Resume { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

        public string? CoverLetter { get; set; }

        public decimal? MatchScore { get; set; }

        public string? RecruiterNotes { get; set; }

        public ICollection<Interview> Interviews { get; set; } = new HashSet<Interview>();
        public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
        public Evaluation? Evaluation { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public string? CompanyFeedback { get; set; }
    }
}