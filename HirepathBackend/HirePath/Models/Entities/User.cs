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

        // NULL for Candidates and Platform Admins.
        // Set for Recruiters / Hiring Managers — this is what determines
        // "their" company for every scoped query in the workflow module.
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        public CandidateProfile? CandidateProfile { get; set; }

        public CompanyMember? CompanyMembership { get; set; }
    }
}