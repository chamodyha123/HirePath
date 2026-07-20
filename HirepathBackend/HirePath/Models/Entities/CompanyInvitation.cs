using System.ComponentModel.DataAnnotations;
using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class CompanyInvitation : BaseEntity
    {
        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        public CompanyMemberRole Role { get; set; }

        [Required]
        [MaxLength(256)]
        public string TokenHash { get; set; } = string.Empty;

        public InvitationStatus Status { get; set; }
            = InvitationStatus.Pending;

        public DateTime ExpiresAt { get; set; }

        public DateTime? AcceptedAt { get; set; }

        public int? InvitedByUserId { get; set; }

        public User? InvitedByUser { get; set; }
    }
}