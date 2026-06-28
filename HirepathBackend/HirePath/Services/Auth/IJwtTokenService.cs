using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Services.Auth
{
    public interface IJwtTokenService
    {
        Task<(string Token, DateTime Expiration)> CreateTokenAsync(User user);
    }
}