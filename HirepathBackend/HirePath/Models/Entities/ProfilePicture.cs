using HirePathAI.API.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePathAI.API.Models.Entities
{
    public class ProfilePicture : BaseEntity
    {
        [Required]
        public int CandidateProfileId { get; set; }

        [ForeignKey(nameof(CandidateProfileId))]
        public CandidateProfile? CandidateProfile { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FileType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadDate { get; set; }

        public bool IsPrimary { get; set; } = true;
    }
}