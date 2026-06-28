using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class PendingRegistration : BaseEntity
    {
        public string FullName { get; set; } = "";

        public string UserName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public string Role { get; set; } = "";

        public DateTime ExpireAt { get; set; }
    }
}