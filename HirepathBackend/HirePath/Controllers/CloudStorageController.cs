using HirePathAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePathAI.API.Controllers
{
    [ApiController]
    [Route("api/cloud-storage")]
    public class CloudStorageController : ControllerBase
    {
        private readonly ICloudStorageService _cloudStorageService;

        public CloudStorageController(ICloudStorageService cloudStorageService)
        {
            _cloudStorageService = cloudStorageService;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var fileUrl = await _cloudStorageService.UploadFileAsync(file, "hirepath/resumes");

            return Ok(new
            {
                Success = true,
                Message = "File uploaded successfully.",
                FileUrl = fileUrl
            });
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(string publicId)
        {
            var deleted = await _cloudStorageService.DeleteFileAsync(publicId);

            if (!deleted)
                return BadRequest("File deletion failed.");

            return Ok("File deleted successfully.");
        }
    }
}