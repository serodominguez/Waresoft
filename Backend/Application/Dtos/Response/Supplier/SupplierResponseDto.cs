namespace Application.Dtos.Response.Supplier
{
    public record SupplierResponseDto
    {
        public int IdSupplier { get; init; }
        public string? CompanyName { get; init; }
        public string? Contact { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Email { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusSupplier { get; init; }
    }
}
