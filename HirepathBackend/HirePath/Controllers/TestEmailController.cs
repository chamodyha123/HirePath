using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/test-email")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("welcome")]
        public async Task<IActionResult> SendWelcome(string email)
        {
            await _emailService.SendWelcomeEmailAsync(
                email,
                "Test User");

            return Ok("Welcome email sent successfully.");
        }

        [HttpPost("otp")]
        public async Task<IActionResult> SendOtp(string email)
        {
            await _emailService.SendOtpEmailAsync(
                email,
                "123456",
                "email verification");

            return Ok("OTP email sent successfully.");
        }

        [HttpPost("interview")]
        public async Task<IActionResult> SendInterview(string email)
        {
            await _emailService.SendInterviewInvitationAsync(
                email,
                "Test Candidate",
                "Software Engineer",
                DateTime.Now.AddDays(2),
                "Online",
                "https://meet.google.com/test-link");

            return Ok("Interview invitation sent successfully.");
        }

        [HttpPost("status")]
        public async Task<IActionResult> SendStatus(string email)
        {
            await _emailService.SendApplicationStatusEmailAsync(
                email,
                "Test Candidate",
                "Software Engineer",
                "Shortlisted",
                "Congratulations. You have been shortlisted for the next stage.");

            return Ok("Status email sent successfully.");
        }
    }
}