namespace Infrastructure.Persistences.ReadModels.Product
{
    public record ProductStatsReadModel
    {
        public int TotalActive { get; init; }
        public int NewThisMonth { get; init; }
    }
}
