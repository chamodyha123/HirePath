namespace HirePathAI.API.DTOs.Candidate
{
    public class CandidateEducationDto
    {
        public int Id { get; set; }
        public string Institute { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Grade { get; set; }
        public string? Description { get; set; }
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
    }
}