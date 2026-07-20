namespace HirePathAI.API.DTOs.Interview
{
    public class UpdateInterviewDto
    {
        public int InterviewId { get; set; }

        // All fields below are optional patches — omitting a field leaves
        // the existing value untouched instead of overwriting it with a default.
        public DateTime? ScheduledAt { get; set; }
        public string? MeetingLink { get; set; }
        public string? Location { get; set; }
        public string? Panel { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
    }
}