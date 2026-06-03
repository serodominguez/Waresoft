namespace Application.Dtos.Response.Product
{
    public record ProductResponseDto
    {
        public int IdProduct { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
        public string? Material { get; init; }
        public string? Color { get; init; }
        public string? UnitMeasure { get; init; }
        public string? Image { get; init; }  
        public int IdBrand { get; init; }
        public string? BrandName { get; init; }
        public int IdCategory { get; init; }
        public string? CategoryName { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusProduct { get; init; }
    }
}
