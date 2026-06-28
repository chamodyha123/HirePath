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

        [HttpPost]
        public async Task<IActionResult> Send()
        {
            await _emailService.SendEmailAsync(
                "peshanchamoth759@gmail.com",
                "HirePath AI Test",
                "<h2>Email Service Working Successfully!</h2>");

            return Ok("Email Sent Successfully");
        }
    }
}