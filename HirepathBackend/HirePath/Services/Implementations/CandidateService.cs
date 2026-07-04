using AutoMapper;
using HirePathAI.API.DTOs.Candidate;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Repositories.Interfaces;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HirePathAI.API.Services.Implementations
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IGenericRepository<CandidateSkill> _skillRepository;
        private readonly IGenericRepository<CandidateEducation> _educationRepository;
        private readonly IGenericRepository<CandidateExperience> _experienceRepository;
        private readonly IGenericRepository<Resume> _resumeRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CandidateService(
            ICandidateRepository candidateRepository,
            IGenericRepository<CandidateSkill> skillRepository,
            IGenericRepository<CandidateEducation> educationRepository,
            IGenericRepository<CandidateExperience> experienceRepository,
            IGenericRepository<Resume> resumeRepository,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _candidateRepository = candidateRepository;
            _skillRepository = skillRepository;
            _educationRepository = educationRepository;
            _experienceRepository = experienceRepository;
            _resumeRepository = resumeRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        // ============ EXISTING METHODS ============

        public async Task<IEnumerable<CandidateProfile>> GetAllAsync()
        {
            return await _candidateRepository.GetAllAsync();
        }

        public async Task<CandidateProfile?> GetProfileAsync(int userId)
        {
            return await _candidateRepository.GetProfileAsync(userId);
        }

        public async Task<CandidateProfile> CreateAsync(CandidateProfile profile)
        {
            await _candidateRepository.AddAsync(profile);
            await _candidateRepository.SaveChangesAsync();
            return profile;
        }

        public async Task<bool> UpdateAsync(CandidateProfile profile)
        {
            var existing = await _candidateRepository.GetByIdAsync(profile.Id);
            if (existing == null)
                return false;

            _candidateRepository.Update(profile);
            await _candidateRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var profile = await _candidateRepository.GetByIdAsync(id);
            if (profile == null)
                return false;

            _candidateRepository.Delete(profile);
            await _candidateRepository.SaveChangesAsync();
            return true;
        }

        // ============ PROFILE MANAGEMENT WITH DTOs ============

        public async Task<CandidateProfileDto> GetProfileDtoAsync(int userId)
        {
            var candidate = await _candidateRepository.GetProfileAsync(userId);
            if (candidate == null)
                throw new Exception("Candidate profile not found");

            return _mapper.Map<CandidateProfileDto>(candidate);
        }

        public async Task<CandidateProfileDto> GetProfileByIdAsync(int candidateId)
        {
            var candidate = await _candidateRepository.GetCandidateWithAllDetailsAsync(candidateId);
            if (candidate == null)
                throw new Exception("Candidate profile not found");

            return _mapper.Map<CandidateProfileDto>(candidate);
        }

        public async Task<CandidateProfileDto> CreateProfileAsync(int userId, CreateCandidateProfileDto dto)
        {
            if (await _candidateRepository.CandidateExistsAsync(userId))
                throw new Exception("Candidate profile already exists");

            var candidate = _mapper.Map<CandidateProfile>(dto);
            candidate.UserId = userId;
            candidate.IsProfileComplete = false;
            candidate.ProfileUpdatedAt = DateTime.UtcNow;
            candidate.CreatedAt = DateTime.UtcNow;

            await _candidateRepository.AddAsync(candidate);
            await _candidateRepository.SaveChangesAsync();

            return _mapper.Map<CandidateProfileDto>(candidate);
        }

        public async Task<CandidateProfileDto> UpdateProfileAsync(int userId, UpdateCandidateProfileDto dto)
        {
            var candidate = await _candidateRepository.GetCandidateByUserIdAsync(userId);
            if (candidate == null)
                throw new Exception("Candidate profile not found");

            _mapper.Map(dto, candidate);
            candidate.ProfileUpdatedAt = DateTime.UtcNow;
            candidate.IsProfileComplete = IsProfileComplete(candidate);

            await _candidateRepository.UpdateAsync(candidate);
            await _candidateRepository.SaveChangesAsync();

            return _mapper.Map<CandidateProfileDto>(candidate);
        }

        public async Task<bool> DeleteProfileAsync(int userId)
        {
            var candidate = await _candidateRepository.GetCandidateByUserIdAsync(userId);
            if (candidate == null)
                return false;

            await _candidateRepository.DeleteAsync(candidate);
            await _candidateRepository.SaveChangesAsync();
            return true;
        }

        // ============ SKILLS MANAGEMENT ============

        public async Task<CandidateSkillDto> AddSkillAsync(int userId, CreateSkillDto dto)
        {
            var candidate = await _candidateRepository.GetCandidateByUserIdAsync(userId);
            if (candidate == null)
                throw new Exception("Candidate profile not found");

            var skill = new CandidateSkill
            {
                CandidateProfileId = candidate.Id,
                SkillName = dto.SkillName,
                SkillLevel = Enum.Parse<SkillLevel>(dto.SkillLevel),
                YearsOfExperience = dto.YearsOfExperience,
                CreatedAt = DateTime.UtcNow
            };

            await _skillRepository.AddAsync(skill);
            await _skillRepository.SaveChangesAsync();

            return _mapper.Map<CandidateSkillDto>(skill);
        }

        public async Task<CandidateSkillDto> UpdateSkillAsync(int skillId, UpdateSkillDto dto)
        {
            var skill = await _skillRepository.GetByIdAsync(skillId);
            if (skill == null)
                throw new Exception("Skill not found");

            skill.SkillName = dto.SkillName;
            skill.SkillLevel = Enum.Parse<SkillLevel>(dto.SkillLevel);
            skill.YearsOfExperience = dto.YearsOfExperience;
            skill.UpdatedAt = DateTime.UtcNow;

            await _skillRepository.UpdateAsync(skill);
            await _skillRepository.SaveChangesAsync();

            return _mapper.Map<CandidateSkillDto>(skill);
        }

        public async Task<bool> DeleteSkillAsync(int skillId)
        {
            var skill = await _skillRepository.GetByIdAsync(skillId);
            if (skill == null)
                return false;

            await _skillRepository.DeleteAsync(skill);
            await _skillRepository.SaveChangesAsync();
            return true;
        }

        // ============ EDUCATION MANAGEMENT ============

        public async Task<CandidateEducationDto> AddEducationAsync(int userId, CreateEducationDto dto)
        {
            var candidate = await _candidateRepository.GetCandidateByUserIdAsync(userId);
            if (candidate == null)
                throw new Exception("Candidate profile not found");

            var education = _mapper.Map<CandidateEducation>(dto);
            education.CandidateProfileId = candidate.Id;
            education.CreatedAt = DateTime.UtcNow;

            await _educationRepository.AddAsync(education);
            await _educationRepository.SaveChangesAsync();

            return _mapper.Map<CandidateEducationDto>(education);
        }

        public async Task<CandidateEducationDto> UpdateEducationAsync(int educationId, UpdateEducationDto dto)
        {
            var education = await _educationRepository.GetByIdAsync(educationId);
            if (education == null)
                throw new Exception("Education not found");

            _mapper.Map(dto, education);
            education.UpdatedAt = DateTime.UtcNow;

            await _educationRepository.UpdateAsync(education);
            await _educationRepository.SaveChangesAsync();

            return _mapper.Map<CandidateEducationDto>(education);
        }

        public async Task<bool> DeleteEducationAsync(int educationId)
        {
            var education = await _educationRepository.GetByIdAsync(educationId);
            if (education == null)
                return false;

            await _educationRepository.DeleteAsync(education);
            await _educationRepository.SaveChangesAsync();
            return true;
        }

        // ============ EXPERIENCE MANAGEMENT ============

        public async Task<CandidateExperienceDto> AddExperienceAsync(int userId, CreateExperienceDto dto)
        {
            var candidate = await _candidateRepository.GetCandidateByUserIdAsync(userId);
            if (candidate == null)
                throw new Exception("Candidate profile not found");

            var experience = _mapper.Map<CandidateExperience>(dto);
            experience.CandidateProfileId = candidate.Id;
            experience.CreatedAt = DateTime.UtcNow;

            await _experienceRepository.AddAsync(experience);
            await _experienceRepository.SaveChangesAsync();

            return _mapper.Map<CandidateExperienceDto>(experience);
        }

        public async Task<CandidateExperienceDto> UpdateExperienceAsync(int experienceId, UpdateExperienceDto dto)
        {
            var experience = await _experienceRepository.GetByIdAsync(experienceId);
            if (experience == null)
                throw new Exception("Experience not found");

            _mapper.Map(dto, experience);
            experience.UpdatedAt = DateTime.UtcNow;

            await _experienceRepository.UpdateAsync(experience);
            await _experienceRepository.SaveChangesAsync();

            return _mapper.Map<CandidateExperienceDto>(experience);
        }

        public async Task<bool> DeleteExperienceAsync(int experienceId)
        {
            var experience = await _experienceRepository.GetByIdAsync(experienceId);
            if (experience == null)
                return false;

            await _experienceRepository.DeleteAsync(experience);
            await _experienceRepository.SaveChangesAsync();
            return true;
        }

        // ============ RESUME MANAGEMENT ============

        public async Task<ResumeDto> UploadResumeAsync(int userId, UploadResumeDto dto)
        {
            var candidate = await _candidateRepository.GetCandidateByUserIdAsync(userId);
            if (candidate == null)
                throw new Exception("Candidate profile not found");

            // Create uploads folder if not exists
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "resumes");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(fileStream);
            }

            // If this is primary, unset other primary resumes
            if (dto.IsPrimary)
            {
                var existingResumes = await _resumeRepository
                    .FindAsync(r => r.CandidateProfileId == candidate.Id);
                foreach (var r in existingResumes)
                {
                    r.IsPrimary = false;
                    await _resumeRepository.UpdateAsync(r);
                }
            }

            var resume = new Resume
            {
                CandidateProfileId = candidate.Id,
                FileName = dto.File.FileName,
                FilePath = $"/resumes/{uniqueFileName}",
                FileType = Path.GetExtension(dto.File.FileName),
                FileSize = dto.File.Length,
                UploadDate = DateTime.UtcNow,
                IsPrimary = dto.IsPrimary,
                CreatedAt = DateTime.UtcNow
            };

            await _resumeRepository.AddAsync(resume);
            await _resumeRepository.SaveChangesAsync();

            return _mapper.Map<ResumeDto>(resume);
        }

        public async Task<bool> DeleteResumeAsync(int resumeId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null)
                return false;

            // Delete physical file
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, resume.FilePath.TrimStart('/'));
            if (File.Exists(filePath))
                File.Delete(filePath);

            await _resumeRepository.DeleteAsync(resume);
            await _resumeRepository.SaveChangesAsync();
            return true;
        }

        public async Task<ResumeDto> SetPrimaryResumeAsync(int resumeId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null)
                throw new Exception("Resume not found");

            // Unset all primary resumes for this candidate
            var candidateResumes = await _resumeRepository
                .FindAsync(r => r.CandidateProfileId == resume.CandidateProfileId);
            foreach (var r in candidateResumes)
            {
                r.IsPrimary = false;
                await _resumeRepository.UpdateAsync(r);
            }

            resume.IsPrimary = true;
            resume.UpdatedAt = DateTime.UtcNow;
            await _resumeRepository.UpdateAsync(resume);
            await _resumeRepository.SaveChangesAsync();

            return _mapper.Map<ResumeDto>(resume);
        }

        public async Task<IEnumerable<ResumeDto>> GetResumesAsync(int userId)
        {
            var candidate = await _candidateRepository.GetCandidateByUserIdAsync(userId);
            if (candidate == null)
                return new List<ResumeDto>();

            var resumes = await _resumeRepository
                .FindAsync(r => r.CandidateProfileId == candidate.Id);

            return _mapper.Map<IEnumerable<ResumeDto>>(resumes);
        }

        // ============ SEARCH ============

        public async Task<IEnumerable<CandidateProfileDto>> SearchCandidatesAsync(string searchTerm)
        {
            var candidates = await _candidateRepository.SearchCandidatesAsync(searchTerm);
            return _mapper.Map<IEnumerable<CandidateProfileDto>>(candidates);
        }

        public async Task<IEnumerable<CandidateProfileDto>> GetCandidatesBySkillAsync(string skill)
        {
            var candidates = await _candidateRepository.GetCandidatesBySkillAsync(skill);
            return _mapper.Map<IEnumerable<CandidateProfileDto>>(candidates);
        }

        // ============ HELPER METHODS ============

        private bool IsProfileComplete(CandidateProfile profile)
        {
            return !string.IsNullOrEmpty(profile.FirstName) &&
                   !string.IsNullOrEmpty(profile.LastName) &&
                   !string.IsNullOrEmpty(profile.Headline) &&
                   !string.IsNullOrEmpty(profile.Summary) &&
                   profile.Skills != null && profile.Skills.Any() &&
                   profile.Educations != null && profile.Educations.Any() &&
                   profile.Resumes != null && profile.Resumes.Any();
        }
    }
}