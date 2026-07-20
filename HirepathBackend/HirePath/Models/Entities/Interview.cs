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

        // Online interviews use MeetingLink; physical interviews use Location
        public string? MeetingLink { get; set; }
        public string? Location { get; set; }

        // Comma-separated names or emails of the interview panel
        public string? Panel { get; set; }
        public string? Notes { get; set; }

        public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;

        // Audit: who scheduled/last modified this interview
        public int ScheduledByUserId { get; set; }
        public User? ScheduledByUser { get; set; }

        // Kept for backward compatibility with AI matching code —
        // real structured feedback now lives in InterviewFeedback
        public string? Feedback { get; set; }
        public decimal? Score { get; set; }
    }
}