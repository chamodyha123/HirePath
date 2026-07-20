using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class Department : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        // Navigation Property
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}