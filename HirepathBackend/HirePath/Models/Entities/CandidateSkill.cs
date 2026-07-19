using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePathAI.API.Models.Entities
{
    public class CandidateSkill : BaseEntity
    {
        [Required]
        public int CandidateProfileId { get; set; }

        [ForeignKey(nameof(CandidateProfileId))]
        public CandidateProfile? CandidateProfile { get; set; }

        [Required]
        [MaxLength(100)]
        public string SkillName { get; set; } = string.Empty;

        public SkillLevel SkillLevel { get; set; } = SkillLevel.Beginner;

        public int? YearsOfExperience { get; set; }  // Added this field
    }
}