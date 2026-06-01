namespace Application.Dtos.Response.Brand
{
    public record BrandSelectResponseDto
    {
        public int IdBrand { get; init; }
        public string? BrandName { get; init; }
    }
}
