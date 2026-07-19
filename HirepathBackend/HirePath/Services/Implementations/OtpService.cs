using System.Security.Cryptography;
using System.Text;
using HirePathAI.API.Data;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Services.Implementations
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public OtpService(
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task GenerateOtpAsync(
            string email,
            OtpPurpose purpose)
        {
            await RemoveExpiredOtpsAsync();

            // Remove previous unused OTPs
            var previousOtps = await _context.EmailOtps
                .Where(x =>
                    x.Email == email &&
                    x.Purpose == purpose &&
                    !x.IsUsed)
                .ToListAsync();

            if (previousOtps.Any())
            {
                _context.EmailOtps.RemoveRange(previousOtps);
            }

            // Generate secure 6-digit OTP
            var otp = RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();

            // Hash OTP
            var hash = HashOtp(otp);

            var emailOtp = new EmailOtp
            {
                Email = email,
                OtpHash = hash,
                Purpose = purpose,
                ExpireAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            _context.EmailOtps.Add(emailOtp);

            await _context.SaveChangesAsync();

            // Determine email purpose
            var purposeText = purpose switch
            {
                OtpPurpose.EmailVerification => "Email Verification",
                OtpPurpose.PasswordReset => "Password Reset",
                _ => "Verification"
            };

            // Send OTP using centralized EmailService
            await _emailService.SendOtpEmailAsync(
                email,
                otp,
                purposeText);
        }

        public async Task<bool> VerifyOtpAsync(
            string email,
            string otp,
            OtpPurpose purpose)
        {
            var hash = HashOtp(otp);

            var record = await _context.EmailOtps
                .Where(x =>
                    x.Email == email &&
                    x.Purpose == purpose &&
                    !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (record == null)
                return false;

            if (record.ExpireAt < DateTime.UtcNow)
                return false;

            if (record.OtpHash != hash)
                return false;

            record.IsUsed = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task ResendOtpAsync(
            string email,
            OtpPurpose purpose)
        {
            await GenerateOtpAsync(email, purpose);
        }

        public async Task RemoveExpiredOtpsAsync()
        {
            var expired = await _context.EmailOtps
                .Where(x => x.ExpireAt < DateTime.UtcNow)
                .ToListAsync();

            if (expired.Any())
            {
                _context.EmailOtps.RemoveRange(expired);
                await _context.SaveChangesAsync();
            }
        }

        private string HashOtp(string otp)
        {
            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(otp);

            var hash = sha.ComputeHash(bytes);

            return Convert.ToHexString(hash);
        }
    }
}