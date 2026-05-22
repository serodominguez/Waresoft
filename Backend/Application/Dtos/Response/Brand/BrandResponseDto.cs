namespace Application.Dtos.Response.Brand
{
    public record BrandResponseDto
    {
        public int IdBrand { get; init; }
        public string? BrandName { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusBrand { get; init; }
    }
}
