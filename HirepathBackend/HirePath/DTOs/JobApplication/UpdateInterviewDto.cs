using System;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.DTOs.JobApplication
{
    public class UpdateInterviewDto
    {
        public int InterviewId { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public InterviewType? InterviewType { get; set; }
        public string? MeetingLink { get; set; }
        public string? Location { get; set; }
        public string? PanelMembers { get; set; }
        public string? Notes { get; set; }
    }
}