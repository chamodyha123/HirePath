using HirePathAI.API.DTOs.Company;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompanyController(ICompanyService service)
        {
            _service = service;
        }

        // CREATE COMPANY
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCompanyDto dto)
        {
            var company = await _service.CreateAsync(dto.Name, dto.Description, dto.Website, dto.Location);
            return Ok(company);
        }

        // GET ALL COMPANIES
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _service.GetAllAsync();
            return Ok(companies);
        }

        // GET COMPANY BY ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _service.GetByIdAsync(id);

            if (company == null)
                return NotFound("Company not found.");

            return Ok(company);
        }

        // ASSIGN A USER (Recruiter/Hiring Manager) TO A COMPANY
        [Authorize(Roles = "Admin")]
        [HttpPut("assign-user")]
        public async Task<IActionResult> AssignUser(AssignUserToCompanyDto dto)
        {
            var assigned = await _service.AssignUserToCompanyAsync(dto.UserId, dto.CompanyId);

            if (!assigned)
                return NotFound("User or company not found.");

            return Ok("User assigned to company successfully.");
        }
    }
}