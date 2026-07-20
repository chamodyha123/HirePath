namespace HirePathAI.DTOs
{
    public class DepartmentCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public int CompanyId { get; set; }
    }

    public class DepartmentResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CompanyId { get; set; }
    }
}