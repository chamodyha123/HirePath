using HirePathAI.API.DTOs.Auth;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Services.Auth;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var allowedRoles = new[] { "Candidate", "Recruiter", "HiringManager" };

            if (!allowedRoles.Contains(dto.Role))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid role."
                };
            }

            if (await _userManager.Users.AnyAsync(x => x.Email == dto.Email))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email already exists."
                };
            }

            if (await _userManager.Users.AnyAsync(x => x.UserName == dto.UserName))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Username already exists."
                };
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.UserName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(" | ", result.Errors.Select(x => x.Description))
                };
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            var roles = await _userManager.GetRolesAsync(user);

            var (token, expiration) =
                await _jwtTokenService.CreateTokenAsync(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Registration Successful",
                Token = token,
                Expiration = expiration,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            User? user =
                await _userManager.FindByEmailAsync(dto.EmailOrUsername);

            if (user == null)
            {
                user = await _userManager.Users.FirstOrDefaultAsync(x =>
                    x.UserName == dto.EmailOrUsername);
            }

            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid credentials."
                };
            }

            var signIn =
                await _signInManager.CheckPasswordSignInAsync(user,
                    dto.Password,
                    false);

            if (!signIn.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid credentials."
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            var (token, expiration) =
                await _jwtTokenService.CreateTokenAsync(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login Successful",
                Token = token,
                Expiration = expiration,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles
            };
        }
    }
}