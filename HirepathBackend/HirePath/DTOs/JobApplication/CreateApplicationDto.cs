namespace HirePathAI.API.DTOs.JobApplication
{
    public class CreateApplicationDto
    {
        public int JobId { get; set; }
        public int CandidateProfileId { get; set; }
        public string? CoverLetter { get; set; }
    }
}