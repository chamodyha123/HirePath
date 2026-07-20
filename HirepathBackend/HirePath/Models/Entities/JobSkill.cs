using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class JobSkill : BaseEntity
    {
        [Required]
        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public Job? Job { get; set; }

        [Required]
        public string SkillName { get; set; } = string.Empty;

        public int PriorityWeight { get; set; } = 1;
    }
}