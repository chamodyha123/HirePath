using Microsoft.AspNetCore.Http;

namespace HirePathAI.API.DTOs.Candidate
{
    public class ResumeDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsPrimary { get; set; }
        public string? ParsedContent { get; set; }
    }

    public class UploadResumeDto
    {
        public IFormFile File { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}