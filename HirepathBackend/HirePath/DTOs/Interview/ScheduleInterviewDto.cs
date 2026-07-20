using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.Interview
{
    public class ScheduleInterviewDto
    {
        [Range(1, int.MaxValue)]
        public int JobApplicationId { get; set; }

        public DateTime ScheduledAt { get; set; }

        [Required, StringLength(30)]
        public string InterviewType { get; set; } = string.Empty;

        [Url, StringLength(500)]
        public string? MeetingLink { get; set; }

        [StringLength(300)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Panel { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }
    }
}
