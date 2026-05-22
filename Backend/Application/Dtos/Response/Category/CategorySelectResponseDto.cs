namespace Application.Dtos.Response.Category
{
    public record CategorySelectResponseDto
    {
        public int IdCategory { get; init; }
        public string? CategoryName { get; init; }
    }
}
