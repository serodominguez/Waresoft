namespace Infrastructure.Persistences.ReadModels.Customer
{
    public record CustomerStatsReadModel
    {
        public int TotalActive { get; init; }
        public int NewThisMonth { get; init; }
        public int NewPreviousMonth { get; init; }
    }
}
