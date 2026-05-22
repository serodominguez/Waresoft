namespace Infrastructure.Persistences.ReadModels.Brand
{
    public record BrandReadModel
    {
        public int Id { get; init; }
        public string? BrandName { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public bool Status { get; init; }
    }
}
