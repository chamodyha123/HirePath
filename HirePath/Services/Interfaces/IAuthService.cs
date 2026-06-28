using HirePathAI.API.DTOs.Auth;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> StartRegistrationAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> VerifyEmailOtpAsync(VerifyEmailOtpDto dto);
        Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto);

        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto dto);
    }
}