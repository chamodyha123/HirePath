using HirePathAI.API.Models.Common;

namespace HirePathAI.API.Models.Entities
{
    public class Resume : BaseEntity
    {
        public int CandidateProfileId { get; set; }
        public CandidateProfile? CandidateProfile { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }
}