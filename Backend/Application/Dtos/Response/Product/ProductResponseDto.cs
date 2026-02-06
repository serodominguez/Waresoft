using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Response.Product
{
    public class ProductResponseDto
    {
        public int IdProduct { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }
        public string? UnitMeasure { get; set; }
        public int IdBrand { get; set; }
        public string? BrandName { get; set; }
        public int IdCategory { get; set; }
        public string? CategoryName { get; set; }
        public string? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusProduct { get; set; }
    }
}
