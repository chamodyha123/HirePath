using System.ComponentModel.DataAnnotations;
using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class Company : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Location { get; set; }

        // Navigation Properties
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}