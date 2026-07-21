using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class Interview : BaseEntity
    {
        public int JobApplicationId { get; set; }
        public JobApplication? JobApplication { get; set; }

        public DateTime ScheduledAt { get; set; }
        public InterviewType InterviewType { get; set; }
        public string? MeetingLink { get; set; }
        public string? Location { get; set; }
        public string? PanelMembers { get; set; }
        public string? Notes { get; set; }
        public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
        public string? Feedback { get; set; }
        public decimal? Score { get; set; }

        // Audit info
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
