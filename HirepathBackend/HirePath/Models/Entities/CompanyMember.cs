using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class CompanyMember : BaseEntity
    {
        public int CompanyId { get; set; }

        public Company Company { get; set; }
            = null!;

        public int UserId { get; set; }

        public User User { get; set; }
            = null!;

        public CompanyMemberRole Role { get; set; }

        public bool IsActive { get; set; } = true;
    }
}