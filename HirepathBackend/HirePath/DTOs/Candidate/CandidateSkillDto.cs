namespace HirePathAI.API.DTOs.Candidate
{
    public class CandidateSkillDto
    {
        public int Id { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string SkillLevel { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
    }

    public class CreateSkillDto
    {
        public string SkillName { get; set; } = string.Empty;
        public string SkillLevel { get; set; } = "Beginner";
        public int? YearsOfExperience { get; set; }
    }

    public class UpdateSkillDto
    {
        public string SkillName { get; set; } = string.Empty;
        public string SkillLevel { get; set; } = "Beginner";
        public int? YearsOfExperience { get; set; }
    }
}