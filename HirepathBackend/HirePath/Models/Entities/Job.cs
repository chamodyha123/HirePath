using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class Job : BaseEntity
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public EmploymentType EmploymentType { get; set; }

        [Required]
        public WorkMode WorkMode { get; set; }

        public string Location { get; set; } = string.Empty;

        [Required]
        public ExperienceLevel ExperienceLevel { get; set; }

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        public int? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        // Navigation Properties
        public ICollection<JobSkill> RequiredSkills { get; set; } = new List<JobSkill>();

        // Note: This links with Member 04's module, keeping it intact
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}