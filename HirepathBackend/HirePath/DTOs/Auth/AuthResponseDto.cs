namespace HirePathAI.API.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }
}