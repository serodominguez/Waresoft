namespace Application.Dtos.Request.Category
{
    public record CategoryRequestDto
    {
        public string? CategoryName { get; init; }
        public string? Description { get; init; }
    }
}
