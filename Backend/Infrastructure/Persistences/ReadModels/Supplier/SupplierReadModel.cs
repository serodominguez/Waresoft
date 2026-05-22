namespace Infrastructure.Persistences.ReadModels.Supplier
{
    public record SupplierReadModel
    {
        public int Id { get; init; }
        public string? CompanyName { get; init; }
        public string? Contact { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Email { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public bool Status { get; init; }
    }
}
