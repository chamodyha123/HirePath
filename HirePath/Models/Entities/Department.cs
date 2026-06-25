using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}