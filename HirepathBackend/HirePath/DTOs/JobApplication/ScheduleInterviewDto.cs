using HirePathAI.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace HirePathAI.API.DTOs.JobApplication
{
    public class ScheduleInterviewDto
    {
        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        public InterviewType InterviewType { get; set; }

        public string? MeetingLink { get; set; }

        public string? Location { get; set; }

        public string? PanelMembers { get; set; }

        public string? Notes { get; set; }

    }
}