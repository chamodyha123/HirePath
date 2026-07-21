using HirePathAI.API.Models.Entities;
using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // ============ CREATE COMPANY ============
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Company company)
        {
            try
            {
                var result = await _companyService.CreateAsync(company);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET ALL COMPANIES ============
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var companies = await _companyService.GetAllAsync();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET COMPANY BY ID ============
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyWithDetailsAsync(id);
                if (company == null)
                    return NotFound(new { error = "Company not found" });

                return Ok(company);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ UPDATE COMPANY ============
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Company company)
        {
            try
            {
                company.Id = id;
                var result = await _companyService.UpdateAsync(company);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ DELETE COMPANY ============
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _companyService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { error = "Company not found" });

                return Ok(new { message = "Company deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ ASSIGN USER TO COMPANY ============
        [Authorize(Roles = "Admin")]
        [HttpPut("assign-user")]
        public async Task<IActionResult> AssignUser([FromBody] AssignUserToCompanyDto dto)
        {
            try
            {
                var result = await _companyService.AssignUserToCompanyAsync(dto.UserId, dto.CompanyId);
                if (!result)
                    return BadRequest(new { error = "User or company not found" });

                return Ok(new { message = "User assigned to company successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ REMOVE USER FROM COMPANY ============
        [Authorize(Roles = "Admin")]
        [HttpPut("remove-user/{userId}")]
        public async Task<IActionResult> RemoveUser(int userId)
        {
            try
            {
                var result = await _companyService.RemoveUserFromCompanyAsync(userId);
                if (!result)
                    return BadRequest(new { error = "User not found" });

                return Ok(new { message = "User removed from company successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ============ GET USERS BY COMPANY ============
        [Authorize(Roles = "Admin,Recruiter,HiringManager")]
        [HttpGet("{companyId}/users")]
        public async Task<IActionResult> GetUsersByCompany(int companyId)
        {
            try
            {
                var users = await _companyService.GetUsersByCompanyAsync(companyId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class AssignUserToCompanyDto
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
    }
}