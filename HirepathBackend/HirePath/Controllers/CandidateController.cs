using HirePathAI.API.DTOs.Candidate;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not authenticated");

            return int.Parse(userIdClaim);
        }

        // ============ PROFILE MANAGEMENT ============

        // GET: api/Candidate
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                var profile = await _candidateService.GetProfileDtoAsync(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/Candidate/{userId}
        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> GetProfileByUserId(int userId)
        {
            try
            {
                var profile = await _candidateService.GetProfileDtoAsync(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/Candidate
        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] CreateCandidateProfileDto dto)
        {
            try
            {
                var userId = GetUserId();
                var profile = await _candidateService.CreateProfileAsync(userId, dto);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Candidate - Full Update (Preserves skills, education, experience, resumes)
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateCandidateProfileDto dto)
        {
            try
            {
                var userId = GetUserId();

                // Get current profile to preserve child entities
                var currentProfileDto = await _candidateService.GetProfileDtoAsync(userId);

                // Map current profile to Update DTO
                var updateDto = new UpdateCandidateProfileDto
                {
                    FirstName = currentProfileDto.FirstName,
                    LastName = currentProfileDto.LastName,
                    Headline = currentProfileDto.Headline,
                    Summary = currentProfileDto.Summary,
                    Location = currentProfileDto.Location,
                    PhoneNumber = currentProfileDto.PhoneNumber,
                    LinkedInUrl = currentProfileDto.LinkedInUrl,
                    PortfolioUrl = currentProfileDto.PortfolioUrl,
                    YearsOfExperience = currentProfileDto.YearsOfExperience
                };

                // Only update fields that are provided (not null/empty)
                if (!string.IsNullOrEmpty(dto.FirstName))
                    updateDto.FirstName = dto.FirstName;

                if (!string.IsNullOrEmpty(dto.LastName))
                    updateDto.LastName = dto.LastName;

                if (!string.IsNullOrEmpty(dto.Headline))
                    updateDto.Headline = dto.Headline;

                if (!string.IsNullOrEmpty(dto.Summary))
                    updateDto.Summary = dto.Summary;

                if (!string.IsNullOrEmpty(dto.Location))
                    updateDto.Location = dto.Location;

                if (!string.IsNullOrEmpty(dto.PhoneNumber))
                    updateDto.PhoneNumber = dto.PhoneNumber;

                if (!string.IsNullOrEmpty(dto.LinkedInUrl))
                    updateDto.LinkedInUrl = dto.LinkedInUrl;

                if (!string.IsNullOrEmpty(dto.PortfolioUrl))
                    updateDto.PortfolioUrl = dto.PortfolioUrl;

                // YearsOfExperience is int (not nullable), so check if it's different from default
                if (dto.YearsOfExperience > 0)
                    updateDto.YearsOfExperience = dto.YearsOfExperience;

                // IMPORTANT: Preserve existing skills, education, experience, resumes
                // DO NOT clear them - they are managed by separate endpoints

                var updatedProfile = await _candidateService.UpdateProfileAsync(userId, updateDto);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: api/Candidate - Partial Update (Recommended for specific field updates)
        [HttpPatch]
        public async Task<IActionResult> PatchProfile([FromBody] JsonPatchDocument<UpdateCandidateProfileDto> patchDoc)
        {
            try
            {
                var userId = GetUserId();

                // Get current profile
                var currentProfileDto = await _candidateService.GetProfileDtoAsync(userId);

                // Map to Update DTO
                var profileDto = new UpdateCandidateProfileDto
                {
                    FirstName = currentProfileDto.FirstName,
                    LastName = currentProfileDto.LastName,
                    Headline = currentProfileDto.Headline,
                    Summary = currentProfileDto.Summary,
                    Location = currentProfileDto.Location,
                    PhoneNumber = currentProfileDto.PhoneNumber,
                    LinkedInUrl = currentProfileDto.LinkedInUrl,
                    PortfolioUrl = currentProfileDto.PortfolioUrl,
                    YearsOfExperience = currentProfileDto.YearsOfExperience
                };

                // Apply patch operations
                patchDoc.ApplyTo(profileDto);

                // Validate the updated DTO
                if (!TryValidateModel(profileDto))
                    return BadRequest(ModelState);

                // Update only the patched fields
                var updatedProfile = await _candidateService.UpdateProfileAsync(userId, profileDto);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Candidate
        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            try
            {
                var userId = GetUserId();
                var result = await _candidateService.DeleteProfileAsync(userId);
                if (result)
                    return NoContent();
                return NotFound(new { message = "Candidate profile not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============ SEARCH ENDPOINTS ============

        // GET: api/Candidate/search?searchTerm=...
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> SearchCandidates([FromQuery] string searchTerm)
        {
            try
            {
                var candidates = await _candidateService.SearchCandidatesAsync(searchTerm);
                return Ok(candidates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Candidate/skill/{skillName}
        [HttpGet("skill/{skillName}")]
        // REMOVED: [Authorize(Roles = "Admin,Recruiter")]
        // Now any authenticated user can access this endpoint
        public async Task<IActionResult> GetCandidatesBySkill(string skillName)
        {
            try
            {
                var candidates = await _candidateService.GetCandidatesBySkillAsync(skillName);
                return Ok(candidates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============ SKILLS MANAGEMENT ============

        // POST: api/Candidate/skills
        [HttpPost("skills")]
        public async Task<IActionResult> AddSkill([FromBody] CreateSkillDto dto)
        {
            try
            {
                var userId = GetUserId();
                var skill = await _candidateService.AddSkillAsync(userId, dto);
                return Ok(skill);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Candidate/skills/{id}
        [HttpPut("skills/{id}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] UpdateSkillDto dto)
        {
            try
            {
                var skill = await _candidateService.UpdateSkillAsync(id, dto);
                return Ok(skill);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Candidate/skills/{id}
        [HttpDelete("skills/{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            try
            {
                var result = await _candidateService.DeleteSkillAsync(id);
                if (result)
                    return NoContent();
                return NotFound(new { message = "Skill not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============ EDUCATION MANAGEMENT ============

        // POST: api/Candidate/education
        [HttpPost("education")]
        public async Task<IActionResult> AddEducation([FromBody] CreateEducationDto dto)
        {
            try
            {
                var userId = GetUserId();
                var education = await _candidateService.AddEducationAsync(userId, dto);
                return Ok(education);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Candidate/education/{id}
        [HttpPut("education/{id}")]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] UpdateEducationDto dto)
        {
            try
            {
                var education = await _candidateService.UpdateEducationAsync(id, dto);
                return Ok(education);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Candidate/education/{id}
        [HttpDelete("education/{id}")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            try
            {
                var result = await _candidateService.DeleteEducationAsync(id);
                if (result)
                    return NoContent();
                return NotFound(new { message = "Education not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============ EXPERIENCE MANAGEMENT ============

        // POST: api/Candidate/experience
        [HttpPost("experience")]
        public async Task<IActionResult> AddExperience([FromBody] CreateExperienceDto dto)
        {
            try
            {
                var userId = GetUserId();
                var experience = await _candidateService.AddExperienceAsync(userId, dto);
                return Ok(experience);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Candidate/experience/{id}
        [HttpPut("experience/{id}")]
        public async Task<IActionResult> UpdateExperience(int id, [FromBody] UpdateExperienceDto dto)
        {
            try
            {
                var experience = await _candidateService.UpdateExperienceAsync(id, dto);
                return Ok(experience);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Candidate/experience/{id}
        [HttpDelete("experience/{id}")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            try
            {
                var result = await _candidateService.DeleteExperienceAsync(id);
                if (result)
                    return NoContent();
                return NotFound(new { message = "Experience not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============ RESUME MANAGEMENT ============

        // GET: api/Candidate/resumes
        [HttpGet("resumes")]
        public async Task<IActionResult> GetResumes()
        {
            try
            {
                var userId = GetUserId();
                var resumes = await _candidateService.GetResumesAsync(userId);
                return Ok(resumes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Candidate/resume
        [HttpPost("resume")]
        public async Task<IActionResult> UploadResume([FromForm] UploadResumeDto dto)
        {
            try
            {
                var userId = GetUserId();
                var resume = await _candidateService.UploadResumeAsync(userId, dto);
                return Ok(resume);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Candidate/resume/{id}
        [HttpDelete("resume/{id}")]
        public async Task<IActionResult> DeleteResume(int id)
        {
            try
            {
                var result = await _candidateService.DeleteResumeAsync(id);
                if (result)
                    return NoContent();
                return NotFound(new { message = "Resume not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Candidate/resume/{id}/primary
        [HttpPut("resume/{id}/primary")]
        public async Task<IActionResult> SetPrimaryResume(int id)
        {
            try
            {
                var resume = await _candidateService.SetPrimaryResumeAsync(id);
                return Ok(resume);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}