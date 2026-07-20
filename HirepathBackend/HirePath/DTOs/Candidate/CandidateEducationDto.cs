namespace HirePathAI.API.DTOs.Candidate
{
    public class CandidateEducationDto
    {
        // ============ EXISTING FIELDS ============
        public int Id { get; set; }
        public string Institute { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Grade { get; set; }
        public string? Description { get; set; }

        // ============ NEW FIELDS ============
        public string? CertificateUrl { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? EducationLevel { get; set; }
        public decimal? GPA { get; set; }
        public decimal? Percentage { get; set; }
        public bool IsVerified { get; set; }
        public string? VerifiedBy { get; set; }
    }

    public class CreateEducationDto
    {
        public string Institute { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Grade { get; set; }
        public string? Description { get; set; }
        public string? CertificateUrl { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? EducationLevel { get; set; }
        public decimal? GPA { get; set; }
        public decimal? Percentage { get; set; }
    }

    public class UpdateEducationDto
    {
        public string Institute { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Grade { get; set; }
        public string? Description { get; set; }
        public string? CertificateUrl { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? EducationLevel { get; set; }
        public decimal? GPA { get; set; }
        public decimal? Percentage { get; set; }
    }
}