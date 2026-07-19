using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class CompanyInvitation
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string TokenHash { get; set; } = string.Empty;

        public InvitationRole Role { get; set; }

        public InvitationStatus Status { get; set; }
            = InvitationStatus.Pending;

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public DateTime? AcceptedAt { get; set; }
    }
}