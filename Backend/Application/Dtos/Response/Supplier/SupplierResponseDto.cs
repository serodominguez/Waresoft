namespace Application.Dtos.Response.Supplier
{
    public class SupplierResponseDto
    {
        public int IdSupplier { get; set; }
        public string? CompanyName { get; set; }
        public string? Contact { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AuditCreateDate { get; set; }
        public string? StatusSupplier { get; set; }
    }
}
