using Microsoft.AspNetCore.Http;

namespace Application.Dtos.Request.Product
{
    public record ProductRequestDto
    {
        public string? Code { get; init; }
        public string? Description { get; init; }
        public string? Material { get; init; }
        public string? Color { get; init; }
        public string? UnitMeasure { get; init; }
        public IFormFile? Image { get; init; }
        public bool RemoveImage { get; init; }
        public int IdBrand { get; init; }
        public int IdCategory { get; init; }
    }
}
