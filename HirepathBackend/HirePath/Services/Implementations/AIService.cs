using System.Text.RegularExpressions;
using HirePathAI.API.Configuration;
using HirePathAI.API.DTOs.AI;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace HirePathAI.API.Services.Implementations
{
    public class AIService : IAIService
    {
        private readonly AISettings _settings;
        private readonly ILogger<AIService> _logger;
        private readonly IJobRepository _jobRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly Dictionary<string, List<string>> _skillSynonyms;

        public AIService(
            IOptions<AISettings> settings,
            ILogger<AIService> logger,
            IJobRepository jobRepository,
            ICandidateRepository candidateRepository,
            IApplicationRepository applicationRepository,
            IEvaluationRepository evaluationRepository)
        {
            _settings = settings.Value;
            _logger = logger;
            _jobRepository = jobRepository;
            _candidateRepository = candidateRepository;
            _applicationRepository = applicationRepository;
            _evaluationRepository = evaluationRepository;
            _skillSynonyms = InitializeSkillSynonyms();
        }

        private Dictionary<string, List<string>> InitializeSkillSynonyms()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["C#"] = new() { "CSharp", "C-Sharp", "DotNet", ".NET" },
                ["JavaScript"] = new() { "JS", "ECMAScript", "ES6", "TypeScript" },
                ["Python"] = new() { "Py", "Python3" },
                ["React"] = new() { "ReactJS", "React.js", "React Native" },
                ["Angular"] = new() { "AngularJS", "Angular 2+" },
                ["Node.js"] = new() { "Node", "NodeJS", "Express.js" },
                ["SQL"] = new() { "MySQL", "PostgreSQL", "MSSQL", "T-SQL" },
                ["Java"] = new() { "J2EE", "Java EE", "Spring", "Spring Boot" },
                ["AWS"] = new() { "Amazon Web Services", "EC2", "S3", "Lambda" },
                ["Azure"] = new() { "Microsoft Azure", "Azure Cloud" },
                ["Docker"] = new() { "Container", "Kubernetes", "K8s" },
                ["Git"] = new() { "GitHub", "GitLab", "Bitbucket" },
                ["MVC"] = new() { "Model-View-Controller", "ASP.NET MVC" },
                ["API"] = new() { "REST API", "RESTful", "Web API", "Microservices" },
                ["Agile"] = new() { "Scrum", "Kanban", "SAFe" },
                ["CI/CD"] = new() { "Jenkins", "GitHub Actions", "GitLab CI" }
            };
        }

        // ============================================================
        // RESUME PARSING - Pure text extraction
        // ============================================================

        public ResumeAnalysisResultDto ParseResume(string resumeText)
        {
            try
            {
                var result = new ResumeAnalysisResultDto
                {
                    FullName = ExtractNameFromText(resumeText),
                    Email = ExtractEmailFromText(resumeText),
                    Phone = ExtractPhoneFromText(resumeText),
                    Summary = ExtractSummaryFromText(resumeText),
                    YearsOfExperience = ExtractYearsFromText(resumeText),
                    Skills = ExtractSkillsFromText(resumeText),
                    SkillDetails = new List<SkillDetailDto>(),
                    Education = ExtractEducationFromText(resumeText),
                    Experience = ExtractExperienceFromText(resumeText),
                    Certifications = new List<string>(),
                    Languages = new List<string>(),
                    Metadata = new Dictionary<string, string>
                    {
                        ["ParsedBy"] = "HirePath AI",
                        ["ParserVersion"] = "1.0"
                    }
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing resume");
                throw;
            }
        }

        public async Task<ResumeParseResponseDto> ParseResumeAsync(ResumeParseRequestDto request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (string.IsNullOrWhiteSpace(request.ResumeText))
                {
                    return new ResumeParseResponseDto
                    {
                        Success = false,
                        ErrorMessage = "Resume text cannot be empty"
                    };
                }

                var result = ParseResume(request.ResumeText);
                stopwatch.Stop();

                return new ResumeParseResponseDto
                {
                    Success = true,
                    Data = result,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing resume");
                return new ResumeParseResponseDto
                {
                    Success = false,
                    ErrorMessage = $"Error parsing resume: {ex.Message}",
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }

        public async Task<ResumeParseResponseDto> ParseResumeFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return new ResumeParseResponseDto { Success = false, ErrorMessage = "No file provided" };

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                var text = await reader.ReadToEndAsync();

                var request = new ResumeParseRequestDto
                {
                    ResumeText = text,
                    FileName = file.FileName,
                    FileType = file.ContentType
                };

                return await ParseResumeAsync(request);
            }
            catch (Exception ex)
            {
                return new ResumeParseResponseDto
                {
                    Success = false,
                    ErrorMessage = $"Error parsing resume file: {ex.Message}"
                };
            }
        }

        // ============================================================
        // TEXT EXTRACTION METHODS
        // ============================================================

        private string ExtractNameFromText(string text)
        {
            var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Take(5))
            {
                var trimmed = line.Trim();
                if (trimmed.Length > 0 && trimmed.Length < 50 && !trimmed.Contains("@") && !trimmed.Contains("Resume") && !trimmed.Contains("CV"))
                {
                    return trimmed;
                }
            }
            return "Unknown";
        }

        private string ExtractEmailFromText(string text)
        {
            var match = Regex.Match(text, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");
            return match.Success ? match.Value : string.Empty;
        }

        private string ExtractPhoneFromText(string text)
        {
            var match = Regex.Match(text, @"(?:\+?\d{1,3}[-.]?)?\(?\d{3}\)?[-.]?\d{3}[-.]?\d{4}");
            return match.Success ? match.Value : string.Empty;
        }

        private string ExtractSummaryFromText(string text)
        {
            var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.Length > 50 && !trimmed.Contains("@") && !trimmed.Contains("http") &&
                    !trimmed.Contains("Skills") && !trimmed.Contains("Experience") && !trimmed.Contains("Education"))
                {
                    return trimmed.Length > 500 ? trimmed.Substring(0, 500) + "..." : trimmed;
                }
            }
            return string.Empty;
        }

        private int ExtractYearsFromText(string text)
        {
            var match = Regex.Match(text, @"(\d+)\s*(?:years?|yrs?)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var years))
                return years;

            var yearsList = new List<int>();
            var dateMatches = Regex.Matches(text, @"(20\d{2}|19\d{2})");
            foreach (Match m in dateMatches)
            {
                if (int.TryParse(m.Value, out var year))
                {
                    yearsList.Add(year);
                }
            }

            if (yearsList.Any())
            {
                var minYear = yearsList.Min();
                var currentYear = DateTime.Now.Year;
                var totalYears = currentYear - minYear;
                return Math.Max(0, totalYears);
            }

            return 0;
        }

        private List<string> ExtractSkillsFromText(string text)
        {
            var skills = new List<string>();
            foreach (var skill in _skillSynonyms.Keys)
            {
                if (text.Contains(skill, StringComparison.OrdinalIgnoreCase))
                    skills.Add(skill);
            }
            return skills.Distinct().ToList();
        }

        private List<EducationExtractedDto> ExtractEducationFromText(string text)
        {
            var education = new List<EducationExtractedDto>();
            var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (Regex.IsMatch(trimmed, @"(?:Bachelor|B\.?S\.?|Master|M\.?S\.?|PhD|Ph\.?D\.?|Associate|High School|University|College|Institute)", RegexOptions.IgnoreCase))
                {
                    var edu = new EducationExtractedDto
                    {
                        Institution = trimmed,
                        Degree = "Unknown",
                        FieldOfStudy = "Unknown"
                    };

                    var degreeMatch = Regex.Match(trimmed, @"(Bachelor|B\.?S\.?|Master|M\.?S\.?|PhD|Ph\.?D\.?|Associate|High School)", RegexOptions.IgnoreCase);
                    if (degreeMatch.Success)
                    {
                        edu.Degree = degreeMatch.Value;
                    }

                    education.Add(edu);
                }
            }

            return education;
        }

        private List<ExperienceExtractedDto> ExtractExperienceFromText(string text)
        {
            var experience = new List<ExperienceExtractedDto>();
            var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            string currentCompany = "";
            string currentTitle = "";
            string currentDescription = "";

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (Regex.IsMatch(trimmed, @"(?:Inc|Corp|LLC|Ltd|Company|Technologies|Solutions|Group)", RegexOptions.IgnoreCase) ||
                    (trimmed.Length > 5 && trimmed.Length < 50 && char.IsUpper(trimmed[0]) && (!trimmed.Contains(" ") || trimmed.Split(' ').Length < 5)))
                {
                    if (!string.IsNullOrEmpty(currentCompany) && !string.IsNullOrEmpty(currentTitle))
                    {
                        experience.Add(new ExperienceExtractedDto
                        {
                            Company = currentCompany,
                            Title = currentTitle,
                            Description = currentDescription,
                            IsCurrent = false
                        });
                    }
                    currentCompany = trimmed;
                    currentTitle = "";
                    currentDescription = "";
                }
                else if (trimmed.Length > 5 && trimmed.Length < 60 && !trimmed.Contains("@") && !trimmed.Contains("http") &&
                         Regex.IsMatch(trimmed, @"(?:Developer|Engineer|Manager|Designer|Analyst|Director|Consultant|Architect|Lead|Senior|Junior|Intern)", RegexOptions.IgnoreCase))
                {
                    currentTitle = trimmed;
                }
                else if (trimmed.Length > 20 && !trimmed.Contains("@") && !trimmed.Contains("http"))
                {
                    if (!string.IsNullOrEmpty(currentDescription))
                        currentDescription += " " + trimmed;
                    else
                        currentDescription = trimmed;
                }
            }

            if (!string.IsNullOrEmpty(currentCompany) && !string.IsNullOrEmpty(currentTitle))
            {
                experience.Add(new ExperienceExtractedDto
                {
                    Company = currentCompany,
                    Title = currentTitle,
                    Description = currentDescription,
                    IsCurrent = false
                });
            }

            return experience;
        }

        // ============================================================
        // SKILL EXTRACTION
        // ============================================================

        public async Task<SkillExtractionResultDto> ExtractSkillsAsync(string text)
        {
            var result = new SkillExtractionResultDto();
            var skills = new List<ExtractedSkillDto>();

            var allSkills = new List<string>();
            foreach (var skill in _skillSynonyms.Keys)
            {
                if (text.Contains(skill, StringComparison.OrdinalIgnoreCase))
                    allSkills.Add(skill);
            }

            var skillPattern = @"\b[A-Z][a-zA-Z#+.]{1,20}\b";
            var matches = Regex.Matches(text, skillPattern);
            foreach (Match match in matches)
            {
                var skill = match.Value;
                if (_skillSynonyms.ContainsKey(skill) || _skillSynonyms.Values.Any(v => v.Contains(skill)))
                    allSkills.Add(skill);
            }

            var groupedSkills = allSkills.GroupBy(s => s).Select(g => new
            {
                Name = g.Key,
                Count = g.Count(),
                Level = DetectSkillLevel(text, g.Key)
            });

            foreach (var skill in groupedSkills)
            {
                skills.Add(new ExtractedSkillDto
                {
                    Name = skill.Name,
                    Level = skill.Level,
                    ConfidenceScore = Math.Min(100, skill.Count * 20),
                    Frequency = skill.Count
                });
            }

            result.Skills = skills.OrderByDescending(s => s.ConfidenceScore).ToList();
            result.TotalSkills = result.Skills.Count;
            result.PrimarySkills = result.Skills.Where(s => s.ConfidenceScore > 70).ToList();
            result.ConfidenceScore = result.Skills.Any() ? result.Skills.Average(s => s.ConfidenceScore) : 0;

            return result;
        }

        public async Task<List<string>> ExtractSkillsFromResumeAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();
            var result = await ExtractSkillsAsync(text);
            return result.Skills.Select(s => s.Name).ToList();
        }

        private string DetectSkillLevel(string text, string skillName)
        {
            var levelPatterns = new Dictionary<string, string[]>
            {
                ["Expert"] = new[] { $"expert {skillName}", $"master {skillName}", $"{skillName} expert" },
                ["Advanced"] = new[] { $"advanced {skillName}", $"proficient {skillName}", $"{skillName} advanced" },
                ["Intermediate"] = new[] { $"intermediate {skillName}", $"moderate {skillName}" },
                ["Beginner"] = new[] { $"beginner {skillName}", $"learning {skillName}", $"basic {skillName}" }
            };

            foreach (var level in levelPatterns)
            {
                foreach (var pattern in level.Value)
                {
                    if (text.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                        return level.Key;
                }
            }
            return "Intermediate";
        }

        // ============================================================
        // JOB MATCHING
        // ============================================================

        public JobMatchResultDto MatchCandidate(Job job, CandidateProfile candidate)
        {
            var jobSkills = job.RequiredSkills?.Select(s => s.SkillName.ToLower()).ToList() ?? new List<string>();
            var candidateSkills = candidate.Skills?.Select(s => s.SkillName.ToLower()).ToList() ?? new List<string>();

            var matchedSkills = jobSkills.Intersect(candidateSkills, StringComparer.OrdinalIgnoreCase).ToList();
            var missingSkills = jobSkills.Except(candidateSkills, StringComparer.OrdinalIgnoreCase).ToList();

            var skillScore = jobSkills.Any() ? (decimal)matchedSkills.Count / jobSkills.Count * 100 : 0;
            var experienceScore = CalculateExperienceMatch(job, candidate);
            var educationScore = CalculateEducationMatch(candidate);
            var locationScore = CalculateLocationMatch(job, candidate);

            var weights = _settings.Ranking;
            var overallScore = (skillScore * weights.SkillWeight) +
                              (experienceScore * weights.ExperienceWeight) +
                              (educationScore * weights.EducationWeight) +
                              (locationScore * weights.LocationWeight);

            return new JobMatchResultDto
            {
                JobId = job.Id,
                CandidateId = candidate.Id,
                JobTitle = job.Title,
                CandidateName = $"{candidate.FirstName} {candidate.LastName}",
                OverallMatchScore = Math.Round(overallScore, 2),
                SkillMatchScore = Math.Round(skillScore, 2),
                ExperienceMatchScore = Math.Round(experienceScore, 2),
                EducationMatchScore = Math.Round(educationScore, 2),
                LocationMatchScore = Math.Round(locationScore, 2),
                MatchedSkills = matchedSkills,
                MissingSkills = missingSkills,
                MatchLevel = overallScore >= 80 ? "Excellent" : overallScore >= 60 ? "Good" : overallScore >= 40 ? "Fair" : "Poor",
                ScoreBreakdown = new Dictionary<string, decimal>
                {
                    ["Skills"] = skillScore,
                    ["Experience"] = experienceScore,
                    ["Education"] = educationScore,
                    ["Location"] = locationScore
                },
                Recommendations = GenerateRecommendations(job, candidate, matchedSkills, missingSkills),
                MatchedAt = DateTime.UtcNow
            };
        }

        public async Task<MatchResponseDto> MatchCandidateAsync(MatchRequestDto request)
        {
            try
            {
                var job = await _jobRepository.GetByIdAsync(request.JobId);
                if (job == null)
                    return new MatchResponseDto { Success = false, ErrorMessage = "Job not found" };

                CandidateProfile candidate = null;
                if (request.CandidateId.HasValue)
                {
                    candidate = await _candidateRepository.GetCandidateWithAllDetailsAsync(request.CandidateId.Value);
                    if (candidate == null)
                        return new MatchResponseDto { Success = false, ErrorMessage = "Candidate not found" };
                }

                if (candidate == null)
                    return new MatchResponseDto { Success = false, ErrorMessage = "No candidate provided" };

                var result = MatchCandidate(job, candidate);
                return new MatchResponseDto { Success = true, Data = result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error matching candidate");
                return new MatchResponseDto { Success = false, ErrorMessage = ex.Message };
            }
        }

        public IEnumerable<CandidateProfile> RankCandidates(Job job, IEnumerable<CandidateProfile> candidates)
        {
            return candidates
                .Select(c => new { Candidate = c, Score = MatchCandidate(job, c).OverallMatchScore })
                .OrderByDescending(x => x.Score)
                .Select(x => x.Candidate);
        }

        public async Task<RankResponseDto> RankCandidatesAsync(RankRequestDto request)
        {
            try
            {
                var job = await _jobRepository.GetByIdAsync(request.JobId);
                if (job == null)
                    return new RankResponseDto { Success = false, ErrorMessage = "Job not found" };

                var results = new List<JobMatchResultDto>();
                var allScores = new List<decimal>();

                foreach (var candidateId in request.CandidateIds)
                {
                    var candidate = await _candidateRepository.GetCandidateWithAllDetailsAsync(candidateId);
                    if (candidate != null)
                    {
                        var matchResult = MatchCandidate(job, candidate);
                        results.Add(matchResult);
                        allScores.Add(matchResult.OverallMatchScore);
                    }
                }

                results = results.OrderByDescending(r => r.OverallMatchScore).ToList();

                var summary = new RankingSummaryDto
                {
                    TotalCandidates = results.Count,
                    AverageScore = allScores.Any() ? allScores.Average() : 0,
                    HighestScore = allScores.Any() ? allScores.Max() : 0,
                    LowestScore = allScores.Any() ? allScores.Min() : 0,
                    TopSkills = results.SelectMany(r => r.MatchedSkills)
                                       .GroupBy(s => s)
                                       .OrderByDescending(g => g.Count())
                                       .Take(10)
                                       .Select(g => g.Key)
                                       .ToList(),
                    CommonMissingSkills = results.SelectMany(r => r.MissingSkills)
                                                 .GroupBy(s => s)
                                                 .OrderByDescending(g => g.Count())
                                                 .Take(5)
                                                 .Select(g => g.Key)
                                                 .ToList(),
                    ScoreDistribution = new Dictionary<string, int>
                    {
                        ["90-100"] = results.Count(r => r.OverallMatchScore >= 90),
                        ["80-89"] = results.Count(r => r.OverallMatchScore >= 80 && r.OverallMatchScore < 90),
                        ["70-79"] = results.Count(r => r.OverallMatchScore >= 70 && r.OverallMatchScore < 80),
                        ["60-69"] = results.Count(r => r.OverallMatchScore >= 60 && r.OverallMatchScore < 70),
                        ["Below 60"] = results.Count(r => r.OverallMatchScore < 60)
                    }
                };

                return new RankResponseDto { Success = true, Candidates = results, Summary = summary };
            }
            catch (Exception ex)
            {
                return new RankResponseDto { Success = false, ErrorMessage = ex.Message };
            }
        }

        // ============================================================
        // JOB RECOMMENDATIONS - UPDATED TO USE GetActiveJobsWithSkillsAsync
        // ============================================================

        public async Task<JobRecommendationResponseDto> GetJobRecommendationsAsync(JobRecommendationRequestDto request)
        {
            try
            {
                var candidate = await _candidateRepository.GetCandidateWithAllDetailsAsync(request.CandidateId);
                if (candidate == null)
                    return new JobRecommendationResponseDto { Success = false, ErrorMessage = "Candidate not found" };

                // ✅ FIX: Use GetActiveJobsWithSkillsAsync to ensure RequiredSkills are loaded
                var allJobs = await _jobRepository.GetActiveJobsWithSkillsAsync();
                var jobList = allJobs.ToList();

                var applications = await _applicationRepository.GetCandidateApplications(candidate.Id);
                var appliedJobIds = applications.Select(a => a.JobId).ToHashSet();

                var recommendations = new List<JobRecommendationDto>();

                foreach (var job in jobList)
                {
                    if (!request.IncludeApplied && appliedJobIds.Contains(job.Id))
                        continue;

                    if (!string.IsNullOrEmpty(request.Location) &&
                        !job.Location.Contains(request.Location, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var matchResult = MatchCandidate(job, candidate);

                    if (matchResult.OverallMatchScore >= _settings.Ranking.MinMatchPercentage)
                    {
                        var isApplied = appliedJobIds.Contains(job.Id);
                        var application = isApplied ? applications.First(a => a.JobId == job.Id) : null;

                        recommendations.Add(new JobRecommendationDto
                        {
                            JobId = job.Id,
                            JobTitle = job.Title,
                            CompanyName = job.Company?.Name ?? "Unknown Company",
                            Location = job.Location,
                            MatchScore = matchResult.OverallMatchScore,
                            MatchReason = $"Matched {matchResult.MatchedSkills.Count} out of {matchResult.MatchedSkills.Count + matchResult.MissingSkills.Count} required skills",
                            WhyThisJob = new List<string>
                            {
                                $"Your skills in {string.Join(", ", matchResult.MatchedSkills.Take(3))} align well with this role",
                                $"Your {candidate.YearsOfExperience} years of experience match the requirements"
                            },
                            SkillsToImprove = matchResult.MissingSkills.Take(3).ToList(),
                            IsApplied = isApplied,
                            ApplicationDate = application?.AppliedDate,
                            PostedDate = job.CreatedAt
                        });
                    }
                }

                recommendations = recommendations
                    .OrderByDescending(r => r.MatchScore)
                    .Take(request.Limit ?? 10)
                    .ToList();

                var summary = new RecommendationSummaryDto
                {
                    TotalRecommendations = recommendations.Count,
                    AverageMatchScore = recommendations.Any() ? recommendations.Average(r => r.MatchScore) : 0,
                    HighestMatchScore = recommendations.Any() ? recommendations.Max(r => r.MatchScore) : 0,
                    JobsByLocation = recommendations.GroupBy(r => r.Location)
                                                    .ToDictionary(g => g.Key, g => g.Count()),
                    TopRecommendedSkills = recommendations.SelectMany(r => r.WhyThisJob).Take(10).ToList()
                };

                return new JobRecommendationResponseDto
                {
                    Success = true,
                    Recommendations = recommendations,
                    Summary = summary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommendations: {Message}", ex.Message);
                return new JobRecommendationResponseDto
                {
                    Success = false,
                    ErrorMessage = $"Error getting recommendations: {ex.Message}"
                };
            }
        }

        // ============================================================
        // RECRUITMENT ANALYTICS
        // ============================================================

        public async Task<RecruitmentAnalyticsResponseDto> GetRecruitmentAnalyticsAsync(RecruitmentAnalyticsRequestDto request)
        {
            try
            {
                var applications = await _applicationRepository.GetByCompanyAsync(request.CompanyId ?? 0);
                var jobs = await _jobRepository.GetAllAsync();

                var appList = applications.ToList();
                var jobList = jobs.ToList();

                if (request.StartDate.HasValue)
                {
                    appList = appList.Where(a => a.AppliedDate >= request.StartDate.Value).ToList();
                    jobList = jobList.Where(j => j.CreatedAt >= request.StartDate.Value).ToList();
                }

                if (request.EndDate.HasValue)
                {
                    appList = appList.Where(a => a.AppliedDate <= request.EndDate.Value).ToList();
                    jobList = jobList.Where(j => j.CreatedAt <= request.EndDate.Value).ToList();
                }

                if (request.DepartmentId.HasValue)
                {
                    appList = appList.Where(a => a.Job?.DepartmentId == request.DepartmentId.Value).ToList();
                    jobList = jobList.Where(j => j.DepartmentId == request.DepartmentId.Value).ToList();
                }

                return new RecruitmentAnalyticsResponseDto
                {
                    Success = true,
                    Overview = CalculateOverview(appList, jobList),
                    Pipeline = CalculatePipeline(appList),
                    Quality = CalculateQuality(appList),
                    Sources = CalculateSources(appList),
                    TimeMetrics = CalculateTimeMetrics(appList),
                    Costs = CalculateCosts(appList),
                    Trends = CalculateTrends(appList),
                    Predictions = CalculatePredictions(appList, jobList)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics");
                return new RecruitmentAnalyticsResponseDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // ============================================================
        // AI REPORTING
        // ============================================================

        public async Task<AIReportResponseDto> GenerateReportAsync(AIReportRequestDto request)
        {
            try
            {
                var analytics = await GetRecruitmentAnalyticsAsync(new RecruitmentAnalyticsRequestDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    CompanyId = request.CompanyId
                });

                if (!analytics.Success)
                {
                    return new AIReportResponseDto
                    {
                        Success = false,
                        ErrorMessage = analytics.ErrorMessage
                    };
                }

                var report = new ReportDataDto
                {
                    ReportId = Guid.NewGuid().ToString(),
                    Title = $"Recruitment Report - {DateTime.UtcNow:yyyy-MM-dd}",
                    GeneratedAt = DateTime.UtcNow,
                    PeriodStart = request.StartDate ?? DateTime.UtcNow.AddDays(-30),
                    PeriodEnd = request.EndDate ?? DateTime.UtcNow,
                    ExecutiveSummary = new ExecutiveSummaryDto
                    {
                        Overview = $"Total applications: {analytics.Overview.TotalApplications}, Hires: {analytics.Overview.HiredCount}",
                        KeyHighlights = new List<string>
                        {
                            $"Application to hire rate: {analytics.Overview.ApplicationToHireRate:F1}%",
                            $"Active jobs: {analytics.Overview.ActiveJobs}"
                        },
                        AreasForImprovement = new List<string>(),
                        OverallHealth = analytics.Overview.ApplicationToHireRate > 10 ? "Good" : "Needs Improvement"
                    },
                    KeyMetrics = new List<KeyMetricDto>
                    {
                        new() { Name = "Total Applications", Value = analytics.Overview.TotalApplications.ToString(), Trend = "stable" },
                        new() { Name = "Hires", Value = analytics.Overview.HiredCount.ToString(), Trend = "up" },
                        new() { Name = "Hire Rate", Value = $"{analytics.Overview.ApplicationToHireRate:F1}%", Trend = "stable" }
                    },
                    Charts = new List<ChartDataDto>(),
                    Insights = new List<InsightDto>(),
                    Recommendations = new List<RecommendationDto>(),
                    Anomalies = new AnomalyDetectionDto { HasAnomalies = false }
                };

                return new AIReportResponseDto
                {
                    Success = true,
                    Report = report,
                    ContentType = "application/json"
                };
            }
            catch (Exception ex)
            {
                return new AIReportResponseDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // ============================================================
        // PRIVATE HELPER METHODS
        // ============================================================

        private decimal CalculateExperienceMatch(Job job, CandidateProfile candidate)
        {
            var jobExperience = (int)job.ExperienceLevel;
            var candidateExperience = candidate.YearsOfExperience;
            if (candidateExperience >= jobExperience) return 100;
            if (candidateExperience == 0) return 0;
            return (decimal)candidateExperience / jobExperience * 100;
        }

        private decimal CalculateEducationMatch(CandidateProfile candidate)
        {
            if (candidate.Educations == null || !candidate.Educations.Any())
                return 50;
            var educationLevels = new Dictionary<string, int>
            {
                ["High School"] = 1,
                ["Associate"] = 2,
                ["Bachelor"] = 3,
                ["Master"] = 4,
                ["PhD"] = 5
            };
            var maxEducation = candidate.Educations.Max(e =>
                educationLevels.ContainsKey(e.Qualification) ? educationLevels[e.Qualification] : 0);
            var requiredEducation = 3;
            return maxEducation >= requiredEducation ? 100 : (decimal)maxEducation / requiredEducation * 100;
        }

        private decimal CalculateLocationMatch(Job job, CandidateProfile candidate)
        {
            if (string.IsNullOrEmpty(job.Location) || string.IsNullOrEmpty(candidate.Location))
                return 50;
            if (job.Location.Equals(candidate.Location, StringComparison.OrdinalIgnoreCase))
                return 100;
            var jobParts = job.Location.Split(',').Select(s => s.Trim().ToLower());
            var candidateParts = candidate.Location.Split(',').Select(s => s.Trim().ToLower());
            return jobParts.Intersect(candidateParts).Any() ? 75 : 25;
        }

        private List<string> GenerateRecommendations(Job job, CandidateProfile candidate, List<string> matchedSkills, List<string> missingSkills)
        {
            var recommendations = new List<string>();
            if (missingSkills.Any())
                recommendations.Add($"Consider gaining experience in: {string.Join(", ", missingSkills.Take(3))}");
            if (candidate.YearsOfExperience < (int)job.ExperienceLevel)
                recommendations.Add($"This role requires {(int)job.ExperienceLevel}+ years. You have {candidate.YearsOfExperience} years.");
            if (!candidate.Educations.Any())
                recommendations.Add("Add your educational background to improve your match score.");
            if (matchedSkills.Count >= 3)
                recommendations.Add($"Your {matchedSkills.Count} matched skills make you a strong candidate.");
            return recommendations;
        }

        private OverviewAnalyticsDto CalculateOverview(List<JobApplication> applications, List<Job> jobs)
        {
            var hired = applications.Count(a => a.Status == ApplicationStatus.Hired);
            var offers = applications.Count(a => a.Status == ApplicationStatus.Offered);
            var interviewsCompleted = applications.Count(a => a.Status == ApplicationStatus.Interviewed);

            return new OverviewAnalyticsDto
            {
                TotalJobs = jobs.Count,
                ActiveJobs = jobs.Count(j => j.IsActive),
                TotalApplications = applications.Count,
                NewApplications = applications.Count(a => a.AppliedDate >= DateTime.UtcNow.AddDays(-7)),
                InterviewsScheduled = interviewsCompleted,
                InterviewsCompleted = interviewsCompleted,
                OffersMade = offers,
                OffersAccepted = hired,
                HiredCount = hired,
                ApplicationToHireRate = applications.Any() ? (decimal)hired / applications.Count * 100 : 0,
                InterviewToHireRate = interviewsCompleted > 0 ? (decimal)hired / interviewsCompleted * 100 : 0,
                LastUpdated = DateTime.UtcNow
            };
        }

        private PipelineAnalyticsDto CalculatePipeline(List<JobApplication> applications)
        {
            var stages = new List<PipelineStageDto>
            {
                new() { Name = "Applied", Count = applications.Count(a => a.Status == ApplicationStatus.Applied) },
                new() { Name = "Under Review", Count = applications.Count(a => a.Status == ApplicationStatus.UnderReview) },
                new() { Name = "Shortlisted", Count = applications.Count(a => a.Status == ApplicationStatus.Shortlisted) },
                new() { Name = "Interview Scheduled", Count = applications.Count(a => a.Status == ApplicationStatus.InterviewScheduled) },
                new() { Name = "Interviewed", Count = applications.Count(a => a.Status == ApplicationStatus.Interviewed) },
                new() { Name = "Offered", Count = applications.Count(a => a.Status == ApplicationStatus.Offered) },
                new() { Name = "Hired", Count = applications.Count(a => a.Status == ApplicationStatus.Hired) },
                new() { Name = "Rejected", Count = applications.Count(a => a.Status == ApplicationStatus.Rejected) }
            };

            var total = applications.Any() ? applications.Count : 1;
            foreach (var stage in stages)
            {
                stage.ConversionRate = (decimal)stage.Count / total * 100;
            }

            return new PipelineAnalyticsDto
            {
                StatusDistribution = stages.ToDictionary(s => s.Name, s => s.Count),
                StageDropoff = stages.ToDictionary(s => s.Name, s => s.Count),
                PipelineStages = stages
            };
        }

        private QualityAnalyticsDto CalculateQuality(List<JobApplication> applications)
        {
            var scores = applications.Where(a => a.MatchScore.HasValue).Select(a => a.MatchScore.Value).ToList();
            return new QualityAnalyticsDto
            {
                AverageMatchScore = scores.Any() ? scores.Average() : 0,
                MedianMatchScore = scores.Any() ? scores.OrderBy(s => s).ElementAt(scores.Count / 2) : 0,
                TopSkills = new Dictionary<string, int>(),
                TopCertifications = new Dictionary<string, int>(),
                CandidateQualityDistribution = new Dictionary<string, int>()
            };
        }

        private SourceAnalyticsDto CalculateSources(List<JobApplication> applications)
        {
            return new SourceAnalyticsDto
            {
                ApplicationsBySource = new Dictionary<string, int> { ["Direct"] = applications.Count },
                HiresBySource = new Dictionary<string, int> { ["Direct"] = applications.Count(a => a.Status == ApplicationStatus.Hired) },
                ConversionBySource = new Dictionary<string, decimal>(),
                CostPerHireBySource = new Dictionary<string, decimal>(),
                TopSources = new List<SourcePerformanceDto>()
            };
        }

        private TimeAnalyticsDto CalculateTimeMetrics(List<JobApplication> applications)
        {
            var hiredApps = applications.Where(a => a.Status == ApplicationStatus.Hired).ToList();
            var daysToHire = hiredApps.Select(a => (DateTime.UtcNow - a.AppliedDate).TotalDays).ToList();

            return new TimeAnalyticsDto
            {
                AverageDaysToHire = daysToHire.Any() ? daysToHire.Average() : 0,
                MedianDaysToHire = daysToHire.Any() ? daysToHire.OrderBy(d => d).ElementAt(daysToHire.Count / 2) : 0,
                MaxDaysToHire = daysToHire.Any() ? daysToHire.Max() : 0,
                MinDaysToHire = daysToHire.Any() ? daysToHire.Min() : 0,
                TimeByStage = new Dictionary<string, double>(),
                TimeByDepartment = new Dictionary<string, double>(),
                TimeByJobLevel = new Dictionary<string, double>()
            };
        }

        private CostAnalyticsDto CalculateCosts(List<JobApplication> applications)
        {
            return new CostAnalyticsDto
            {
                AverageCostPerHire = 0,
                TotalRecruitmentCost = 0,
                CostBySource = new Dictionary<string, decimal>(),
                CostByDepartment = new Dictionary<string, decimal>(),
                CostByJobLevel = new Dictionary<string, decimal>()
            };
        }

        private List<TrendAnalyticsDto> CalculateTrends(List<JobApplication> applications)
        {
            var trends = new List<TrendAnalyticsDto>();
            var grouped = applications.GroupBy(a => a.AppliedDate.Date);

            foreach (var group in grouped.OrderBy(g => g.Key))
            {
                var apps = group.ToList();
                trends.Add(new TrendAnalyticsDto
                {
                    Date = group.Key,
                    Applications = apps.Count,
                    Interviews = apps.Count(a => a.Status == ApplicationStatus.Interviewed),
                    Hires = apps.Count(a => a.Status == ApplicationStatus.Hired),
                    AverageMatchScore = apps.Where(a => a.MatchScore.HasValue).Select(a => a.MatchScore.Value)
                                           .DefaultIfEmpty(0).Average(),
                    ActiveJobs = 1
                });
            }

            return trends;
        }

        private PredictiveAnalyticsDto CalculatePredictions(List<JobApplication> applications, List<Job> jobs)
        {
            return new PredictiveAnalyticsDto
            {
                PredictedHiresNextMonth = 5,
                PredictedApplicationsNextMonth = 50,
                PredictedSkillsDemand = new Dictionary<string, int>(),
                PredictedHiresByDepartment = new Dictionary<string, int>(),
                SuccessProbability = new Dictionary<string, decimal>()
            };
        }
    }
}