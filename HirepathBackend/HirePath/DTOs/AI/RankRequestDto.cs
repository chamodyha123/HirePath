using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.DTOs.AI
{
    public class RankRequestDto
    {
        public Job Job { get; set; } = null!;
        public List<CandidateProfile> Candidates { get; set; } = new();
    }
}