namespace Application.Dtos.Response.Category
{
    public class CategoryResponseDto
    {
        public int IdCategory { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public string? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusCategory { get; set; }
    }
}
