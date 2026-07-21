using HirePathAI.API.DTOs.CompanyOnboarding;

namespace HirePathAI.API.Services.CompanyOnboarding
{
    public interface ICompanyOnboardingService
    {
        Task<object> SubmitRegistrationAsync(
            SubmitCompanyRegistrationDto dto);

        Task<IEnumerable<object>> GetRegistrationRequestsAsync(
            string? status);

        Task<object> ApproveRegistrationAsync(
            int requestId,
            int platformAdminUserId,
            string? note);

        Task<object> RejectRegistrationAsync(
            int requestId,
            int platformAdminUserId,
            string? note);

        Task<object> InviteMemberAsync(
            int companyAdminUserId,
            InviteCompanyMemberDto dto);

        Task<object> ValidateInvitationAsync(
            string token);

        Task<object> AcceptInvitationAsync(
            AcceptCompanyInvitationDto dto);
    }
}