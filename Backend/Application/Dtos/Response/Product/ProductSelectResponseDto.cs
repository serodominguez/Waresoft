namespace Application.Dtos.Response.Product
{
    public record ProductSelectResponseDto
    {
        public int IdProduct { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
    }
}
