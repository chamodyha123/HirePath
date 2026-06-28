using HirePathAI.API.Models.Common;
using HirePathAI.API.Models.Enums;

namespace HirePathAI.API.Models.Entities
{
    public class EmailOtp : BaseEntity
    {
        public string Email { get; set; } = string.Empty;

        // Store hashed OTP instead of plain text
        public string OtpHash { get; set; } = string.Empty;

        public OtpPurpose Purpose { get; set; }

        public DateTime ExpireAt { get; set; }

        public bool IsUsed { get; set; } = false;
    }
}