namespace HirePathAI.API.DTOs.JobApplication
{
    public class CreateApplicationDto
    {
        public int JobId { get; set; }

        // Optional — if omitted, the candidate's primary resume is used.
        public int? ResumeId { get; set; }

        public string? CoverLetter { get; set; }
    }
}