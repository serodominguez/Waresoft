namespace Application.Dtos.Request.Product
{
    public class ProductRequestDto
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }
        public string? UnitMeasure { get; set; }
        public int IdBrand { get; set; }
        public int IdCategory { get; set; }
    }
}
