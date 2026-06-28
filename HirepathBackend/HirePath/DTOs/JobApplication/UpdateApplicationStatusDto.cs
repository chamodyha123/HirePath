using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.DTOs.JobApplication
{
    public class UpdateApplicationStatusDto
    {
        public int ApplicationId { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? Feedback { get; set; }
    }
}