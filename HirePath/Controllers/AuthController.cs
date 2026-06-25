using HirePathAI.API.DTOs.Auth;
using HirePathAI.API.Models.Entities;
using HirePathAI.API.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var allowedRoles = new[] { "Candidate", "Recruiter", "HiringManager" };

            if (!allowedRoles.Contains(dto.Role))
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid role selected."
                });
            }

            var existingEmail = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingEmail != null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email is already in use."
                });
            }

            var existingUsername = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if (existingUsername != null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Username is already in use."
                });
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.UserName,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(" | ", createResult.Errors.Select(e => e.Description))
                });
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiration) = await _jwtTokenService.CreateTokenAsync(user);

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Registration successful.",
                Token = token,
                Expiration = expiration,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User? user = await _userManager.FindByEmailAsync(dto.EmailOrUsername);

            if (user == null)
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName == dto.EmailOrUsername);
            }

            if (user == null)
            {
                return Unauthorized(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid email/username or password."
                });
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!signInResult.Succeeded)
            {
                return Unauthorized(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid email/username or password."
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiration) = await _jwtTokenService.CreateTokenAsync(user);

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login successful.",
                Token = token,
                Expiration = expiration,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles
            });
        }
    }
}