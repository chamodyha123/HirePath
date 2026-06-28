using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class CandidateSkill : BaseEntity
    {
        public int CandidateProfileId { get; set; }
        public CandidateProfile? CandidateProfile { get; set; }

        public string SkillName { get; set; } = string.Empty;
        public SkillLevel SkillLevel { get; set; } = SkillLevel.Beginner;
    }
}