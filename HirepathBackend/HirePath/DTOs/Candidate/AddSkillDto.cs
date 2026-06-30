using System.ComponentModel.DataAnnotations;
using HirePathAI.API.Models.Enums;  

namespace HirePathAI.API.DTOs.Candidate 
{
    public class AddSkillDto
    {
        [Required(ErrorMessage = "Skill name is required")]
        [StringLength(50, ErrorMessage = "Skill name cannot exceed 50 characters")]
        public required string SkillName { get; set; }

        [Required(ErrorMessage = "Skill level is required")]
        public SkillLevel Level { get; set; }
    }
}