using HirePathAI.API.Data;
using HirePathAI.API.DTOs.Auth;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Models.Enums;
using HirePathAI.API.Services.Auth;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IOtpService _otpService;

        public AuthService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService,
            IOtpService otpService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _otpService = otpService;
        }

        public async Task<AuthResponseDto> StartRegistrationAsync(RegisterRequestDto dto)
        {
            var allowedRoles = new[] { "Candidate" };

            if (!allowedRoles.Contains(dto.Role))
                return Fail("Only candidates can self-register. Company staff must use a secure invitation.");

            if (await _userManager.Users.AnyAsync(x => x.Email == dto.Email))
                return Fail("Email already exists.");

            if (await _userManager.Users.AnyAsync(x => x.UserName == dto.UserName))
                return Fail("Username already exists.");

            var existingPending = await _context.PendingRegistrations
                .Where(x => x.Email == dto.Email || x.UserName == dto.UserName)
                .ToListAsync();

            if (existingPending.Any())
            {
                _context.PendingRegistrations.RemoveRange(existingPending);
                await _context.SaveChangesAsync();
            }

            var pending = new PendingRegistration
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role,
                ExpireAt = DateTime.UtcNow.AddMinutes(5)
            };

            _context.PendingRegistrations.Add(pending);
            await _context.SaveChangesAsync();

            await _otpService.GenerateOtpAsync(dto.Email, OtpPurpose.EmailVerification);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Registration started. Verification OTP sent to your email."
            };
        }

        public async Task<AuthResponseDto> VerifyEmailOtpAsync(VerifyEmailOtpDto dto)
        {
            var otpValid = await _otpService.VerifyOtpAsync(
                dto.Email,
                dto.Otp,
                OtpPurpose.EmailVerification);

            if (!otpValid)
                return Fail("Invalid or expired OTP.");

            var pending = await _context.PendingRegistrations
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (pending == null)
                return Fail("Registration request not found.");

            if (pending.ExpireAt < DateTime.UtcNow)
            {
                _context.PendingRegistrations.Remove(pending);
                await _context.SaveChangesAsync();

                return Fail("Registration request expired. Please register again.");
            }

            var user = new User
            {
                FullName = pending.FullName,
                UserName = pending.UserName,
                Email = pending.Email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, pending.Password);

            if (!createResult.Succeeded)
                return Fail(string.Join(" | ", createResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, pending.Role);

            _context.PendingRegistrations.Remove(pending);
            await _context.SaveChangesAsync();

            return await BuildAuthResponseAsync(user, "Email verified successfully. Account created.");
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var request = new RegisterRequestDto
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                Password = dto.Password,
                ConfirmPassword = dto.Password,
                Role = dto.Role
            };

            return await StartRegistrationAsync(request);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            User? user = await _userManager.FindByEmailAsync(dto.EmailOrUsername);

            if (user == null)
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(x => x.UserName == dto.EmailOrUsername);
            }

            if (user == null)
                return Fail("Invalid credentials.");

            if (!user.EmailConfirmed)
                return Fail("Please verify your email before logging in.");

            var signIn = await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                false);

            if (!signIn.Succeeded)
                return Fail("Invalid credentials.");

            return await BuildAuthResponseAsync(user, "Login successful.");
        }

        public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return Fail("Account not found.");

            await _otpService.GenerateOtpAsync(dto.Email, OtpPurpose.PasswordReset);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Password reset OTP sent to your email."
            };
        }

        public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var otpValid = await _otpService.VerifyOtpAsync(
                dto.Email,
                dto.Otp,
                OtpPurpose.PasswordReset);

            if (!otpValid)
                return Fail("Invalid or expired OTP.");

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return Fail("Account not found.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                resetToken,
                dto.NewPassword);

            if (!result.Succeeded)
                return Fail(string.Join(" | ", result.Errors.Select(e => e.Description)));

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Password reset successfully."
            };
        }

        private async Task<AuthResponseDto> BuildAuthResponseAsync(User user, string message)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiration) = await _jwtTokenService.CreateTokenAsync(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = message,
                Token = token,
                Expiration = expiration,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles
            };
        }

        private static AuthResponseDto Fail(string message)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}