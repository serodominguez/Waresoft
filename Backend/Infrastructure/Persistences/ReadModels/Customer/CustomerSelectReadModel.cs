namespace Infrastructure.Persistences.ReadModels.Customer
{
    public record CustomerSelectReadModel
    {
        public int Id { get; init; }
        public string? Names { get; init; }
        public string? LastNames { get; init; }
        public bool Status { get; init; }
    }
}
