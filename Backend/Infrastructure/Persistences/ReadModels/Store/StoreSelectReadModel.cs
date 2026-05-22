namespace Infrastructure.Persistences.ReadModels.Store
{
    public record StoreSelectReadModel
    {
        public int Id { get; init; }
        public string? StoreName { get; init; }
        public bool Status { get; init; }
    }
}
