using HirePathAI.API.DTOs.Candidate;
using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Interfaces
{
    public interface ICandidateService
    {
        // Existing methods
        Task<CandidateProfile?> GetProfileAsync(int userId);
        Task<IEnumerable<CandidateProfile>> GetAllAsync();
        Task<CandidateProfile> CreateAsync(CandidateProfile profile);
        Task<bool> UpdateAsync(CandidateProfile profile);
        Task<bool> DeleteAsync(int id);

        // Profile Management with DTOs
        Task<CandidateProfileDto> GetProfileDtoAsync(int userId);
        Task<CandidateProfileDto> GetProfileByIdAsync(int candidateId);
        Task<CandidateProfileDto> CreateProfileAsync(int userId, CreateCandidateProfileDto dto);
        Task<CandidateProfileDto> UpdateProfileAsync(int userId, UpdateCandidateProfileDto dto);
        Task<bool> DeleteProfileAsync(int userId);

        // Skills Management
        Task<CandidateSkillDto> AddSkillAsync(int userId, CreateSkillDto dto);
        Task<CandidateSkillDto> UpdateSkillAsync(int skillId, UpdateSkillDto dto);
        Task<bool> DeleteSkillAsync(int skillId);

        // Education Management
        Task<CandidateEducationDto> AddEducationAsync(int userId, CreateEducationDto dto);
        Task<CandidateEducationDto> UpdateEducationAsync(int educationId, UpdateEducationDto dto);
        Task<bool> DeleteEducationAsync(int educationId);

        // Experience Management
        Task<CandidateExperienceDto> AddExperienceAsync(int userId, CreateExperienceDto dto);
        Task<CandidateExperienceDto> UpdateExperienceAsync(int experienceId, UpdateExperienceDto dto);
        Task<bool> DeleteExperienceAsync(int experienceId);

        // Resume Management
        Task<ResumeDto> UploadResumeAsync(int userId, UploadResumeDto dto);
        Task<bool> DeleteResumeAsync(int resumeId);
        Task<ResumeDto> SetPrimaryResumeAsync(int resumeId);
        Task<IEnumerable<ResumeDto>> GetResumesAsync(int userId);

        // ============ PROFILE PICTURE MANAGEMENT ============
        Task<ProfilePictureDto> UploadProfilePictureAsync(int userId, UploadProfilePictureDto dto);
        Task<bool> DeleteProfilePictureAsync(int userId);
        Task<ProfilePictureDto> GetProfilePictureAsync(int userId);

        // Search
        Task<IEnumerable<CandidateProfileDto>> SearchCandidatesAsync(string searchTerm);
        Task<IEnumerable<CandidateProfileDto>> GetCandidatesBySkillAsync(string skill);
    }
}