namespace Infrastructure.Persistences.ReadModels.Module
{
    public record ModuleReadModel
    {
        public int Id { get; init; }
        public string? ModuleName { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public bool IsActive { get; init; }
    }
}
