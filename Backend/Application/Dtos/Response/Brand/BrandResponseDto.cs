namespace Application.Dtos.Response.Brand
{
    public class BrandResponseDto
    {
        public int IdBrand { get; set; }
        public string? BrandName { get; set; }
        public string? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusBrand { get; set; }
    }
}
