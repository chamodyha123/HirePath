using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HirePathAI.API.Services.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public JwtTokenService(
            IConfiguration configuration,
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
        }

        public async Task<(string Token, DateTime Expiration)> CreateTokenAsync(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");

            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var durationInMinutes = int.Parse(jwtSection["DurationInMinutes"] ?? "60");

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new Claim("fullName", user.FullName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var membership = await _context.CompanyMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.IsActive);

            if (membership != null)
            {
                claims.Add(new Claim("companyId", membership.CompanyId.ToString()));
                claims.Add(new Claim("companyRole", membership.Role.ToString()));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(durationInMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expiration);
        }
    }
}