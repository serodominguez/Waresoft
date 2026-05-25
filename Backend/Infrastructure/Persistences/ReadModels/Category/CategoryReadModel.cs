namespace Infrastructure.Persistences.ReadModels.Category
{
    public record CategoryReadModel
    {
        public int Id { get; init; }
        public string? CategoryName { get; init; }
        public string? Description { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public bool IsActive { get; init; }
    }
}
