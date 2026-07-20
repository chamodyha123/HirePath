namespace HirePathAI.API.DTOs.PlatformAdmin.Users
{
    public class PlatformUserResponseDto
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int? CompanyId { get; set; }
    }
}