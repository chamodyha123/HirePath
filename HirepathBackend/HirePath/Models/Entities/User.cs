using Microsoft.AspNetCore.Identity;

namespace HirePathAI.API.Models.Entities
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public CandidateProfile? CandidateProfile { get; set; }
    }
}