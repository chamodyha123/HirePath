using HirePathAI.API.DTOs.AI;
using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IAIService
    {
        ResumeAnalysisResultDto ParseResume(string resume);

        JobMatchResultDto MatchCandidate(Job job, CandidateProfile candidate);

        IEnumerable<CandidateProfile> RankCandidates(
            Job job,
            IEnumerable<CandidateProfile> candidates);
    }
}