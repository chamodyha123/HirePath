using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HirePathAI.API.Models.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public CandidateProfile? CandidateProfile { get; set; }

        public CompanyMember? CompanyMembership { get; set; }
    }
}