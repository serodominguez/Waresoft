namespace Application.Dtos.Response.Supplier
{
    public record SupplierSelectResponseDto
    {
        public int IdSupplier { get; init; }
        public string? CompanyName { get; init; }
    }
}
