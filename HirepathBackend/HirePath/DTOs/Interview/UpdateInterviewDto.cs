namespace HirePathAI.API.DTOs.Interview
{
    public class UpdateInterviewDto
    {
        public int InterviewId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string? MeetingLink { get; set; }
        public string? Status { get; set; }
    }
}