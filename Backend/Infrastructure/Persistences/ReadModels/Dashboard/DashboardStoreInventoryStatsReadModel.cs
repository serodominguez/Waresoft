namespace Infrastructure.Persistences.ReadModels.Dashboard
{
    public record DashboardStoreInventoryStatsReadModel
    {
        public int BelowMinimum { get; init; }
        public int BelowMinimumThisMonth { get; init; }
        public int BelowMinimumLastMonth { get; init; }
    }
}
