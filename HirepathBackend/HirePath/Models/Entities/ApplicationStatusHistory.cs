using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    // One row per status transition — gives the full audit trail
    // (who changed what, and when) that the spec requires.
    public class ApplicationStatusHistory : BaseEntity
    {
        public int JobApplicationId { get; set; }
        public JobApplication? JobApplication { get; set; }

        public ApplicationStatus FromStatus { get; set; }
        public ApplicationStatus ToStatus { get; set; }

        public int ChangedByUserId { get; set; }
        public User? ChangedByUser { get; set; }

        public string? Notes { get; set; }

        // CreatedAt (from BaseEntity) is the timestamp of this change
    }
}