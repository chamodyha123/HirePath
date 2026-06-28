namespace HirePathAI.API.DTOs.AI
{
    public class ResumeAnalysisResultDto
    {
        public List<string> Skills { get; set; } = new();

        public int YearsOfExperience { get; set; }

        public string Summary { get; set; } = string.Empty;
    }
}