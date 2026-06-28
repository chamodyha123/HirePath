using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.DTOs.AI
{
    public class MatchRequestDto
    {
        public Job Job { get; set; } = null!;
        public CandidateProfile Candidate { get; set; } = null!;
    }
}