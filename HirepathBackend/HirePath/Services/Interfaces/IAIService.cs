// HirePathAI.API/Services/Interfaces/IAIService.cs
using HirePathAI.API.DTOs.AI;
using HirePathAI.API.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IAIService
    {
        // ============ RESUME PARSING ============
        ResumeAnalysisResultDto ParseResume(string resumeText);
        Task<ResumeParseResponseDto> ParseResumeAsync(ResumeParseRequestDto request);
        Task<ResumeParseResponseDto> ParseResumeFileAsync(IFormFile file);

        // ============ MATCHING ============
        JobMatchResultDto MatchCandidate(Job job, CandidateProfile candidate);
        Task<MatchResponseDto> MatchCandidateAsync(MatchRequestDto request);
        IEnumerable<CandidateProfile> RankCandidates(Job job, IEnumerable<CandidateProfile> candidates);
        Task<RankResponseDto> RankCandidatesAsync(RankRequestDto request);

        // ============ RECOMMENDATIONS ============
        Task<JobRecommendationResponseDto> GetJobRecommendationsAsync(JobRecommendationRequestDto request);

        // ============ SKILL EXTRACTION ============
        Task<SkillExtractionResultDto> ExtractSkillsAsync(string text);
        Task<List<string>> ExtractSkillsFromResumeAsync(IFormFile file);

        // ============ ANALYTICS & REPORTING ============
        Task<RecruitmentAnalyticsResponseDto> GetRecruitmentAnalyticsAsync(RecruitmentAnalyticsRequestDto request);
        Task<AIReportResponseDto> GenerateReportAsync(AIReportRequestDto request);
    }
}