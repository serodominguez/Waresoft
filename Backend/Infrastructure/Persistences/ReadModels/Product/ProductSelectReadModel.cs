namespace Infrastructure.Persistences.ReadModels.Product
{
    public record ProductSelectReadModel
    {
        public int Id { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; }
    }
}
