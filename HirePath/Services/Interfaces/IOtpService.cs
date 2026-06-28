using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IOtpService
    {
        Task GenerateOtpAsync(string email, OtpPurpose purpose);

        Task<bool> VerifyOtpAsync(
            string email,
            string otp,
            OtpPurpose purpose);

        Task ResendOtpAsync(
            string email,
            OtpPurpose purpose);

        Task RemoveExpiredOtpsAsync();
    }
}