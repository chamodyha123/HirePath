using HirePathAI.API.DTOs.AI;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Services.Interfaces;

namespace HirePathAI.API.Services.Implementations
{
    public class AIService : IAIService
    {
        private readonly List<string> KnownSkills =
        [
            "C#",
            "ASP.NET",
            "Java",
            "Python",
            "SQL",
            "React",
            "Angular",
            "JavaScript",
            "Docker",
            "Azure",
            "AWS",
            "Git",
            "HTML",
            "CSS"
        ];

        public ResumeAnalysisResultDto ParseResume(string resume)
        {
            var result = new ResumeAnalysisResultDto();

            foreach (var skill in KnownSkills)
            {
                if (resume.Contains(skill, StringComparison.OrdinalIgnoreCase))
                {
                    result.Skills.Add(skill);
                }
            }

            result.YearsOfExperience = 2;

            result.Summary =
                $"Detected {result.Skills.Count} technical skills.";

            return result;
        }

        public JobMatchResultDto MatchCandidate(
            Job job,
            CandidateProfile candidate)
        {
            var result = new JobMatchResultDto();

            var required = job.RequiredSkills
                .Select(x => x.SkillName)
                .ToList();

            var candidateSkills = candidate.Skills
                .Select(x => x.SkillName)
                .ToList();

            result.MatchedSkills =
                required.Intersect(candidateSkills).ToList();

            result.MissingSkills =
                required.Except(candidateSkills).ToList();

            if (required.Count == 0)
            {
                result.MatchScore = 0;
            }
            else
            {
                result.MatchScore =
                    Math.Round(
                        (decimal)result.MatchedSkills.Count /
                        required.Count * 100,
                        2);
            }

            return result;
        }

        public IEnumerable<CandidateProfile> RankCandidates(
            Job job,
            IEnumerable<CandidateProfile> candidates)
        {
            return candidates
                .OrderByDescending(candidate =>
                    MatchCandidate(job, candidate).MatchScore)
                .ToList();
        }
    }
}