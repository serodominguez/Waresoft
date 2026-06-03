namespace Application.Dtos.Request.Supplier
{
    public record SupplierRequestDto
    {
        public string? CompanyName { get; init; }
        public string? Contact { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Email { get; init; }
    }
}
