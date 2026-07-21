using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HirePathAI.API.Data;
using HirePathAI.API.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Controllers;

[ApiController]
[Route("api/company/profile")]
[Authorize(Roles = "CompanyAdmin")]
public class CompanyProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CompanyProfileController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var companyId = await GetCompanyIdAsync();
        var company = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == companyId);
        return company == null ? NotFound(new { message = "Company not found." }) : Ok(company);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCompanyProfileDto dto)
    {
        var companyId = await GetCompanyIdAsync();
        var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == companyId);
        if (company == null) return NotFound(new { message = "Company not found." });
        company.Name = dto.Name.Trim();
        company.Industry = Clean(dto.Industry);
        company.Email = Clean(dto.Email);
        company.Phone = Clean(dto.Phone);
        company.Address = Clean(dto.Address);
        company.Description = Clean(dto.Description);
        company.Website = Clean(dto.Website);
        company.Location = Clean(dto.Location);
        company.LogoUrl = Clean(dto.LogoUrl);
        company.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Company profile updated successfully.", company });
    }

    private async Task<int> GetCompanyIdAsync()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(value, out var userId)) throw new UnauthorizedAccessException("Invalid user token.");
        var membership = await _context.CompanyMembers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Role == CompanyMemberRole.CompanyAdmin && x.IsActive);
        if (membership == null) throw new UnauthorizedAccessException("Active Company Admin membership not found.");
        return membership.CompanyId;
    }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public class UpdateCompanyProfileDto
{
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [MaxLength(100)] public string? Industry { get; set; }
    [EmailAddress, MaxLength(150)] public string? Email { get; set; }
    [Phone, MaxLength(30)] public string? Phone { get; set; }
    [MaxLength(300)] public string? Address { get; set; }
    [MaxLength(1000)] public string? Description { get; set; }
    [Url, MaxLength(250)] public string? Website { get; set; }
    [MaxLength(150)] public string? Location { get; set; }
    [Url, MaxLength(500)] public string? LogoUrl { get; set; }
}
