using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class Job : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public EmploymentType EmploymentType { get; set; }
        public WorkMode WorkMode { get; set; }
        public string Location { get; set; } = string.Empty;
        public ExperienceLevel ExperienceLevel { get; set; }

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public bool IsActive { get; set; } = true;

        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public ICollection<JobSkill> RequiredSkills { get; set; } = new List<JobSkill>();
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}