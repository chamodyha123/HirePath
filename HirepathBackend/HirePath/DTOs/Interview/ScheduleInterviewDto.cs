namespace HirePathAI.API.DTOs.Interview
{
	public class ScheduleInterviewDto
	{
		public int JobApplicationId { get; set; }
		public DateTime ScheduledAt { get; set; }
		public string InterviewType { get; set; } = string.Empty;
		public string? MeetingLink { get; set; }
	}
}