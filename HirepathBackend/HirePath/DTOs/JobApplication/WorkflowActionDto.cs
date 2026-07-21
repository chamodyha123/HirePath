using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.DTOs.JobApplication
{
    public class WorkflowActionDto
    {
        public int ApplicationId { get; set; }
        public string? Notes { get; set; }

        // For interview scheduling
        public DateTime? ScheduledDate { get; set; }
        public InterviewType? InterviewType { get; set; }
        public string? MeetingLink { get; set; }
        public string? InterviewPanel { get; set; }

        // For offer
        public decimal? OfferSalary { get; set; }
        public DateTime? OfferExpiryDate { get; set; }
        public string? OfferDetails { get; set; }
    }
}
