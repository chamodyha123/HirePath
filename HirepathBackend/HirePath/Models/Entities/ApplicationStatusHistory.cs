using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class ApplicationStatusHistory : BaseEntity
    {
        public int ApplicationId { get; set; }
        public JobApplication? Application { get; set; }

        public ApplicationStatus Status { get; set; }
        public string? Notes { get; set; }
        public int ChangedByUserId { get; set; }
        public User? ChangedByUser { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}