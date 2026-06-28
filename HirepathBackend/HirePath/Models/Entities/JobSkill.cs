using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class JobSkill : BaseEntity
    {
        public int JobId { get; set; }
        public Job? Job { get; set; }

        public string SkillName { get; set; } = string.Empty;
        public int PriorityWeight { get; set; } = 1;
    }
}