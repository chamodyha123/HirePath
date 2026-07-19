using HirePathAI.API.Models.Enums;

namespace HirePathAI.DTOs
{
    public class JobCreateDTO
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
        public int CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }

    public class JobResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string WorkMode { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public string? CompanyName { get; set; }
        public string? DepartmentName { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }
}