namespace HirePathAI.API.DTOs.AI
{
    public class JobMatchResultDto
    {
        public decimal MatchScore { get; set; }

        public List<string> MatchedSkills { get; set; } = new();

        public List<string> MissingSkills { get; set; } = new();
    }
}