namespace Infrastructure.Persistences.ReadModels.Customer
{
    public record CustomerReadModel
    {
        public int Id { get; init; }
        public string? Names { get; init; }
        public string? LastNames { get; init; }
        public string? IdentificationNumber { get; init; }
        public string? PhoneNumber { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public bool Status { get; init; }
    }
}
