namespace Application.Dtos.Response.Category
{
    public record CategoryResponseDto
    {
        public int IdCategory { get; init; }
        public string? CategoryName { get; init; }
        public string? Description { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusCategory { get; init; }
    }
}
