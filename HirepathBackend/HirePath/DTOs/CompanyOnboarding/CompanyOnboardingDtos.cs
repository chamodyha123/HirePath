using System.ComponentModel.DataAnnotations;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.DTOs.CompanyOnboarding
{
    public class SubmitCompanyRegistrationDto
    {
        [Required] public string CompanyName { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? BusinessRegistrationNumber { get; set; }
        [Required, EmailAddress] public string CompanyEmail { get; set; } = string.Empty;
        public string? CompanyPhone { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        [Required] public string RepresentativeName { get; set; } = string.Empty;
        [Required, EmailAddress] public string RepresentativeEmail { get; set; } = string.Empty;
        public string? RepresentativePhone { get; set; }
    }

    public class ReviewCompanyRegistrationDto
    {
        public string? Note { get; set; }
    }

    public class InviteCompanyMemberDto
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string FullName { get; set; } = string.Empty;
        [Required] public CompanyMemberRole Role { get; set; }
    }

    public class AcceptCompanyInvitationDto
    {
        [Required] public string Token { get; set; } = string.Empty;
        [Required] public string UserName { get; set; } = string.Empty;
        [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
        [Required, Compare(nameof(Password))] public string ConfirmPassword { get; set; } = string.Empty;
    }
}
