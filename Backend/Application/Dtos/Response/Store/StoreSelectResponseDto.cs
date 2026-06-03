namespace Application.Dtos.Response.Store
{
    public record StoreSelectResponseDto
    {
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
    }
}
