namespace Infrastructure.Persistences.ReadModels.Store
{
    public record StoreReadModel
    {
        public int Id { get; init; }
        public string? StoreName { get; init; }
        public string? Manager { get; init; }
        public string? Address { get; init; }
        public string? PhoneNumber { get; init; }
        public string? City { get; init; }
        public string? Email { get; init; }
        public decimal ProfitMargin { get; init; }
        public string? Type { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public bool IsActive { get; init; }
    }
}
