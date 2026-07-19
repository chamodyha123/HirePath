using Microsoft.AspNetCore.Http;

namespace HirePathAI.API.Services.Interfaces
{
    public interface ICloudStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        Task<bool> DeleteFileAsync(string publicId);
    }
}