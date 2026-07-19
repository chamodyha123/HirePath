using HirePathAI.DTOs;
using HirePathAI.API.Models.Entities;
using HirePathAI.Repositories;

namespace HirePathAI.Services
{
    public class RecruiterService : IRecruiterService
    {
        private readonly IRecruiterRepository _repository;

        public RecruiterService(IRecruiterRepository repository)
        {
            _repository = repository;
        }

        public async Task<CompanyResponseDTO> AddCompanyAsync(CompanyCreateDTO dto)
        {
            var company = new Company
            {
                Name = dto.Name, Industry = dto.Industry, Email = dto.Email, Phone = dto.Phone,
                Address = dto.Address, Description = dto.Description, Website = dto.Website,
                Location = dto.Location, LogoUrl = dto.LogoUrl, Status = dto.Status
            };
            var result = await _repository.CreateCompanyAsync(company);
            return MapCompany(result);
        }

        public async Task<IEnumerable<CompanyResponseDTO>> GetAllCompaniesAsync()
        {
            var companies = await _repository.GetAllCompaniesAsync();
            return companies.Select(MapCompany);
        }

        public async Task<DepartmentResponseDTO> AddDepartmentAsync(DepartmentCreateDTO dto)
        {
            var dept = new Department { Name = dto.Name, CompanyId = dto.CompanyId };
            var result = await _repository.CreateDepartmentAsync(dept);
            return new DepartmentResponseDTO { Id = result.Id, Name = result.Name, CompanyId = result.CompanyId };
        }

        public async Task<IEnumerable<DepartmentResponseDTO>> GetDepartmentsAsync(int companyId)
        {
            var depts = await _repository.GetDepartmentsByCompanyAsync(companyId);
            return depts.Select(d => new DepartmentResponseDTO { Id = d.Id, Name = d.Name, CompanyId = d.CompanyId });
        }

        public async Task<JobResponseDTO> PostJobAsync(JobCreateDTO dto)
        {
            var job = new Job
            {
                Title = dto.Title,
                Description = dto.Description,
                EmploymentType = dto.EmploymentType,
                WorkMode = dto.WorkMode,
                Location = dto.Location,
                ExperienceLevel = dto.ExperienceLevel,
                SalaryMin = dto.SalaryMin,
                SalaryMax = dto.SalaryMax,
                ApplicationDeadline = dto.ApplicationDeadline,
                CompanyId = dto.CompanyId,
                DepartmentId = dto.DepartmentId
            };

            var createdJob = await _repository.CreateJobAsync(job);

            if (dto.Skills != null && dto.Skills.Any())
            {
                var skills = dto.Skills.Select(s => new JobSkill { SkillName = s, JobId = createdJob.Id });
                await _repository.AddJobSkillsAsync(skills);
            }

            return await GetJobDetailsAsync(createdJob.Id) ?? new JobResponseDTO();
        }

        public async Task<JobResponseDTO?> GetJobDetailsAsync(int id)
        {
            var job = await _repository.GetJobByIdAsync(id);
            if (job == null) return null;

            return new JobResponseDTO
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                EmploymentType = job.EmploymentType.ToString(),
                WorkMode = job.WorkMode.ToString(),
                Location = job.Location,
                ExperienceLevel = job.ExperienceLevel.ToString(),
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                ApplicationDeadline = job.ApplicationDeadline,
                CompanyName = job.Company?.Name,
                DepartmentName = job.Department?.Name,
                Skills = job.RequiredSkills.Select(s => s.SkillName).ToList()
            };
        }

        public async Task<IEnumerable<JobResponseDTO>> SearchJobsAsync(string? search, string? location)
        {
            var jobs = await _repository.GetAllJobsAsync(search, location);
            return jobs.Select(job => new JobResponseDTO
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                EmploymentType = job.EmploymentType.ToString(),
                WorkMode = job.WorkMode.ToString(),
                Location = job.Location,
                ExperienceLevel = job.ExperienceLevel.ToString(),
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                ApplicationDeadline = job.ApplicationDeadline,
                CompanyName = job.Company?.Name,
                DepartmentName = job.Department?.Name,
                Skills = job.RequiredSkills.Select(s => s.SkillName).ToList()
            });
        }

        public async Task<JobResponseDTO?> EditJobAsync(int id, JobCreateDTO dto)
        {
            var job = await _repository.GetJobByIdAsync(id);
            if (job == null) return null;

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.EmploymentType = dto.EmploymentType;
            job.WorkMode = dto.WorkMode;
            job.Location = dto.Location;
            job.ExperienceLevel = dto.ExperienceLevel;
            job.SalaryMin = dto.SalaryMin;
            job.SalaryMax = dto.SalaryMax;
            job.ApplicationDeadline = dto.ApplicationDeadline;
            job.CompanyId = dto.CompanyId;
            job.DepartmentId = dto.DepartmentId;

            await _repository.UpdateJobAsync(job);
            return await GetJobDetailsAsync(id);
        }

        public async Task<bool> RemoveJobAsync(int id)
        {
            return await _repository.DeleteJobAsync(id);
        }

        public async Task<object> GetDashboardStatsAsync()
        {
            var jobs = await _repository.GetAllJobsAsync(null, null);
            var companies = await _repository.GetAllCompaniesAsync();

            return new
            {
                TotalJobsPosted = jobs.Count(),
                ActiveJobs = jobs.Count(j => j.IsActive),
                TotalCompaniesMapped = companies.Count()
            };
        }
        private static CompanyResponseDTO MapCompany(Company c) => new()
        {
            Id = c.Id, Name = c.Name, Industry = c.Industry, Email = c.Email, Phone = c.Phone,
            Address = c.Address, Description = c.Description, Website = c.Website,
            Location = c.Location, LogoUrl = c.LogoUrl, Status = c.Status.ToString()
        };

    }
}