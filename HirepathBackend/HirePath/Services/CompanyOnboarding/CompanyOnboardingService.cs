using System.Security.Cryptography;
using System.Text;
using HirePathAI.API.Data;
using HirePathAI.API.DTOs.CompanyOnboarding;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Services.CompanyOnboarding
{
    public class CompanyOnboardingService : ICompanyOnboardingService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public CompanyOnboardingService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<object> SubmitRegistrationAsync(
            SubmitCompanyRegistrationDto dto)
        {
            string companyEmail =
                dto.CompanyEmail.Trim().ToLowerInvariant();

            string representativeEmail =
                dto.RepresentativeEmail.Trim().ToLowerInvariant();

            bool duplicate =
                await _context.CompanyRegistrationRequests.AnyAsync(x =>
                    x.Status == CompanyRegistrationStatus.Pending &&
                    (
                        x.CompanyEmail == companyEmail ||
                        x.RepresentativeEmail == representativeEmail
                    ));

            if (duplicate)
            {
                throw new InvalidOperationException(
                    "A pending company registration already exists for this email.");
            }

            var request =
                new CompanyRegistrationRequest
                {
                    CompanyName = dto.CompanyName.Trim(),
                    Industry = dto.Industry?.Trim(),
                    BusinessRegistrationNumber =
                        dto.BusinessRegistrationNumber?.Trim(),
                    CompanyEmail = companyEmail,
                    CompanyPhone = dto.CompanyPhone?.Trim(),
                    Address = dto.Address?.Trim(),
                    Website = dto.Website?.Trim(),
                    Description = dto.Description?.Trim(),
                    LogoUrl = dto.LogoUrl?.Trim(),

                    RepresentativeName =
                        dto.RepresentativeName.Trim(),

                    RepresentativeEmail =
                        representativeEmail,

                    RepresentativePhone =
                        dto.RepresentativePhone?.Trim(),

                    Status =
                        CompanyRegistrationStatus.Pending
                };

            _context.CompanyRegistrationRequests.Add(request);

            await _context.SaveChangesAsync();

            return new
            {
                request.Id,
                status = request.Status.ToString(),
                message =
                    "Company registration submitted for Platform Admin approval."
            };
        }

        public async Task<IEnumerable<object>>
            GetRegistrationRequestsAsync(
                string? status)
        {
            IQueryable<CompanyRegistrationRequest> query =
                _context.CompanyRegistrationRequests
                    .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(status))
            {
                bool validStatus =
                    Enum.TryParse(
                        status,
                        true,
                        out CompanyRegistrationStatus parsedStatus);

                if (!validStatus)
                {
                    throw new ArgumentException(
                        "Invalid company registration status.",
                        nameof(status));
                }

                query =
                    query.Where(x =>
                        x.Status == parsedStatus);
            }

            var requests =
                await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new
                    {
                        x.Id,
                        x.CompanyName,
                        x.Industry,
                        x.BusinessRegistrationNumber,
                        x.CompanyEmail,
                        x.CompanyPhone,
                        x.Address,
                        x.Website,
                        x.Description,
                        x.LogoUrl,
                        x.RepresentativeName,
                        x.RepresentativeEmail,
                        x.RepresentativePhone,
                        Status = x.Status.ToString(),
                        x.ReviewNote,
                        x.CreatedAt,
                        x.UpdatedAt,
                        x.ReviewedAt,
                        x.ReviewedByUserId,
                        x.CreatedCompanyId
                    })
                    .ToListAsync();

            return requests.Cast<object>();
        }

        public async Task<object> ApproveRegistrationAsync(
            int requestId,
            int platformAdminUserId,
            string? note)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var request =
                    await _context.CompanyRegistrationRequests
                        .FirstOrDefaultAsync(x =>
                            x.Id == requestId)
                    ?? throw new KeyNotFoundException(
                        "Company registration request not found.");

                if (request.Status !=
                    CompanyRegistrationStatus.Pending)
                {
                    throw new InvalidOperationException(
                        "This company registration request has already been reviewed.");
                }

                User? existingUser =
                    await _userManager.FindByEmailAsync(
                        request.RepresentativeEmail);

                if (existingUser != null)
                {
                    throw new InvalidOperationException(
                        "The representative email already belongs to an existing account.");
                }

                bool companyEmailExists =
                    await _context.Companies.AnyAsync(x =>
                        x.Email == request.CompanyEmail);

                if (companyEmailExists)
                {
                    throw new InvalidOperationException(
                        "A company with this email already exists.");
                }

                var company =
                    new Company
                    {
                        Name =
                            request.CompanyName,

                        Industry =
                            request.Industry,

                        Email =
                            request.CompanyEmail,

                        Phone =
                            request.CompanyPhone,

                        Address =
                            request.Address,

                        Website =
                            request.Website,

                        Description =
                            request.Description,

                        LogoUrl =
                            request.LogoUrl,

                        Location =
                            request.Address,

                        Status =
                            CompanyStatus.Approved
                    };

                _context.Companies.Add(company);

                await _context.SaveChangesAsync();

                string rawToken =
                    CreateRawToken();

                var invitation =
                    new CompanyInvitation
                    {
                        CompanyId =
                            company.Id,

                        Company =
                            company,

                        Email =
                            request.RepresentativeEmail,

                        FullName =
                            request.RepresentativeName,

                        Role =
                            CompanyMemberRole.CompanyAdmin,

                        TokenHash =
                            HashToken(rawToken),

                        ExpiresAt =
                            DateTime.UtcNow.AddHours(48),

                        Status =
                            InvitationStatus.Pending,

                        InvitedByUserId =
                            platformAdminUserId
                    };

                _context.CompanyInvitations.Add(invitation);

                request.Status =
                    CompanyRegistrationStatus.Approved;

                request.ReviewNote =
                    note?.Trim();

                request.ReviewedAt =
                    DateTime.UtcNow;

                request.ReviewedByUserId =
                    platformAdminUserId;

                request.CreatedCompanyId =
                    company.Id;

                request.UpdatedAt =
                    DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                await SendInvitationEmailAsync(
                    invitation,
                    rawToken);

                return new
                {
                    company.Id,
                    company.Name,
                    status = company.Status.ToString(),
                    invitation.Email,
                    role = invitation.Role.ToString(),
                    invitation.ExpiresAt,
                    message =
                        "Company approved and Company Admin activation invitation sent."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<object> RejectRegistrationAsync(
            int requestId,
            int platformAdminUserId,
            string? note)
        {
            var request =
                await _context.CompanyRegistrationRequests
                    .FirstOrDefaultAsync(x =>
                        x.Id == requestId)
                ?? throw new KeyNotFoundException(
                    "Company registration request not found.");

            if (request.Status !=
                CompanyRegistrationStatus.Pending)
            {
                throw new InvalidOperationException(
                    "This company registration request has already been reviewed.");
            }

            request.Status =
                CompanyRegistrationStatus.Rejected;

            request.ReviewNote =
                note?.Trim();

            request.ReviewedAt =
                DateTime.UtcNow;

            request.ReviewedByUserId =
                platformAdminUserId;

            request.UpdatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new
            {
                request.Id,
                status = request.Status.ToString(),
                message =
                    "Company registration rejected."
            };
        }

        public async Task<object> InviteMemberAsync(
            int companyAdminUserId,
            InviteCompanyMemberDto dto)
        {
            if (dto.Role != CompanyMemberRole.Recruiter &&
                dto.Role != CompanyMemberRole.HiringManager)
            {
                throw new InvalidOperationException(
                    "Company Admins can invite only Recruiters or Hiring Managers.");
            }

            var adminMembership =
                await _context.CompanyMembers
                    .Include(x => x.Company)
                    .FirstOrDefaultAsync(x =>
                        x.UserId == companyAdminUserId &&
                        x.Role ==
                            CompanyMemberRole.CompanyAdmin &&
                        x.IsActive)
                ?? throw new UnauthorizedAccessException(
                    "An active Company Admin membership was not found.");

            if (adminMembership.Company.Status !=
                CompanyStatus.Approved)
            {
                throw new InvalidOperationException(
                    "The company is not active.");
            }

            string normalizedEmail =
                dto.Email.Trim().ToLowerInvariant();

            User? existingUser =
                await _userManager.FindByEmailAsync(
                    normalizedEmail);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    "This email already belongs to an existing user.");
            }

            var pendingInvitations =
                await _context.CompanyInvitations
                    .Where(x =>
                        x.Email == normalizedEmail &&
                        x.Status ==
                            InvitationStatus.Pending)
                    .ToListAsync();

            foreach (var pendingInvitation
                     in pendingInvitations)
            {
                pendingInvitation.Status =
                    InvitationStatus.Revoked;

                pendingInvitation.UpdatedAt =
                    DateTime.UtcNow;
            }

            string rawToken =
                CreateRawToken();

            var invitation =
                new CompanyInvitation
                {
                    CompanyId =
                        adminMembership.CompanyId,

                    Company =
                        adminMembership.Company,

                    Email =
                        normalizedEmail,

                    FullName =
                        dto.FullName.Trim(),

                    Role =
                        dto.Role,

                    TokenHash =
                        HashToken(rawToken),

                    ExpiresAt =
                        DateTime.UtcNow.AddHours(48),

                    Status =
                        InvitationStatus.Pending,

                    InvitedByUserId =
                        companyAdminUserId
                };

            _context.CompanyInvitations.Add(invitation);

            await _context.SaveChangesAsync();

            await SendInvitationEmailAsync(
                invitation,
                rawToken);

            return new
            {
                invitation.Id,
                invitation.Email,
                role = invitation.Role.ToString(),
                invitation.ExpiresAt,
                message =
                    "Invitation sent successfully."
            };
        }

        public async Task<object> ValidateInvitationAsync(
            string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(
                    "Invitation token is required.",
                    nameof(token));
            }

            string hash =
                HashToken(token);

            var invitation =
                await _context.CompanyInvitations
                    .Include(x => x.Company)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.TokenHash == hash)
                ?? throw new KeyNotFoundException(
                    "Invitation not found.");

            if (invitation.Status !=
                InvitationStatus.Pending)
            {
                throw new InvalidOperationException(
                    "This invitation is no longer valid.");
            }

            if (invitation.ExpiresAt <=
                DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    "This invitation has expired.");
            }

            if (invitation.Company.Status !=
                CompanyStatus.Approved)
            {
                throw new InvalidOperationException(
                    "The company is not active.");
            }

            return new
            {
                invitation.Email,
                invitation.FullName,
                role = invitation.Role.ToString(),
                companyId = invitation.CompanyId,
                companyName = invitation.Company.Name,
                invitation.ExpiresAt
            };
        }

        public async Task<object> AcceptInvitationAsync(
            AcceptCompanyInvitationDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                throw new InvalidOperationException(
                    "Password and confirmation password do not match.");
            }

            string hash =
                HashToken(dto.Token);

            var invitation =
                await _context.CompanyInvitations
                    .Include(x => x.Company)
                    .FirstOrDefaultAsync(x =>
                        x.TokenHash == hash)
                ?? throw new KeyNotFoundException(
                    "Invitation not found.");

            if (invitation.Status !=
                InvitationStatus.Pending)
            {
                throw new InvalidOperationException(
                    "This invitation is no longer valid.");
            }

            if (invitation.ExpiresAt <=
                DateTime.UtcNow)
            {
                invitation.Status =
                    InvitationStatus.Expired;

                invitation.UpdatedAt =
                    DateTime.UtcNow;

                await _context.SaveChangesAsync();

                throw new InvalidOperationException(
                    "This invitation has expired.");
            }

            if (invitation.Company.Status !=
                CompanyStatus.Approved)
            {
                throw new InvalidOperationException(
                    "The company is not active.");
            }

            User? existingEmailUser =
                await _userManager.FindByEmailAsync(
                    invitation.Email);

            if (existingEmailUser != null)
            {
                throw new InvalidOperationException(
                    "An account already exists for this email.");
            }

            User? existingUsernameUser =
                await _userManager.FindByNameAsync(
                    dto.UserName);

            if (existingUsernameUser != null)
            {
                throw new InvalidOperationException(
                    "Username already exists.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var user =
                    new User
                    {
                        FullName =
                            invitation.FullName,

                        UserName =
                            dto.UserName.Trim(),

                        Email =
                            invitation.Email,

                        EmailConfirmed =
                            true,

                        IsActive =
                            true
                    };

                IdentityResult createResult =
                    await _userManager.CreateAsync(
                        user,
                        dto.Password);

                if (!createResult.Succeeded)
                {
                    string errors =
                        string.Join(
                            " | ",
                            createResult.Errors.Select(
                                x => x.Description));

                    throw new InvalidOperationException(
                        errors);
                }

                string identityRole =
                    invitation.Role.ToString();

                bool roleExists =
                    await _context.Roles.AnyAsync(x =>
                        x.Name == identityRole);

                if (!roleExists)
                {
                    throw new InvalidOperationException(
                        $"Identity role '{identityRole}' does not exist. Add it in SeedData before accepting invitations.");
                }

                IdentityResult roleResult =
                    await _userManager.AddToRoleAsync(
                        user,
                        identityRole);

                if (!roleResult.Succeeded)
                {
                    string errors =
                        string.Join(
                            " | ",
                            roleResult.Errors.Select(
                                x => x.Description));

                    throw new InvalidOperationException(
                        errors);
                }

                var companyMember =
                    new CompanyMember
                    {
                        CompanyId =
                            invitation.CompanyId,

                        UserId =
                            user.Id,

                        Role =
                            invitation.Role,

                        IsActive =
                            true
                    };

                _context.CompanyMembers.Add(
                    companyMember);

                invitation.Status =
                    InvitationStatus.Accepted;

                invitation.AcceptedAt =
                    DateTime.UtcNow;

                invitation.UpdatedAt =
                    DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                await _emailService.SendWelcomeEmailAsync(
                    user.Email!,
                    user.FullName);

                return new
                {
                    user.Id,
                    user.Email,
                    user.UserName,
                    companyId = invitation.CompanyId,
                    companyName =
                        invitation.Company.Name,
                    role =
                        invitation.Role.ToString(),
                    message =
                        "Invitation accepted and account activated."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task SendInvitationEmailAsync(
            CompanyInvitation invitation,
            string rawToken)
        {
            string frontendUrl =
                _configuration["FrontendUrl"]?
                    .TrimEnd('/')
                ?? "http://localhost:5173";

            string activationLink =
                $"{frontendUrl}/accept-invitation" +
                $"?token={Uri.EscapeDataString(rawToken)}";

            string safeName =
                System.Net.WebUtility.HtmlEncode(
                    invitation.FullName);

            string safeCompanyName =
                System.Net.WebUtility.HtmlEncode(
                    invitation.Company?.Name ??
                    "your company");

            string safeLink =
                System.Net.WebUtility.HtmlEncode(
                    activationLink);

            string htmlBody =
                $"""
                <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto;">
                    <h2>HirePath AI Invitation</h2>

                    <p>Hello {safeName},</p>

                    <p>
                        You have been invited to join
                        <strong>{safeCompanyName}</strong>
                        as a
                        <strong>{invitation.Role}</strong>.
                    </p>

                    <p>
                        Click the button below to activate your account.
                    </p>

                    <p style="margin:24px 0;">
                        <a href="{safeLink}"
                           style="
                               background:#2563eb;
                               color:white;
                               padding:12px 20px;
                               text-decoration:none;
                               border-radius:6px;
                               display:inline-block;">
                            Activate Account
                        </a>
                    </p>

                    <p>
                        This invitation expires on
                        {invitation.ExpiresAt:u}.
                    </p>

                    <p>
                        If you were not expecting this invitation,
                        you can ignore this email.
                    </p>
                </div>
                """;

            await _emailService.SendEmailAsync(
                invitation.Email,
                "Activate your HirePath AI account",
                htmlBody);
        }

        private static string CreateRawToken()
        {
            byte[] tokenBytes =
                RandomNumberGenerator.GetBytes(32);

            return System.Convert.ToHexString(
                tokenBytes);
        }

        private static string HashToken(
            string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(
                    "Invitation token cannot be empty.",
                    nameof(token));
            }

            byte[] tokenBytes =
                Encoding.UTF8.GetBytes(
                    token.Trim());

            byte[] hashBytes =
                SHA256.HashData(tokenBytes);

            return System.Convert.ToHexString(
                hashBytes);
        }
    }
}