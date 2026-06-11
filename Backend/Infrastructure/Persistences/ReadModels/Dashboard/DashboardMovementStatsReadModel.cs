namespace Infrastructure.Persistences.ReadModels.Dashboard
{
    public record DashboardMovementStatsReadModel
    {
        public int Month { get; init; }
        public int Year { get; init; }
        public decimal TotalReceipts { get; init; }
        public decimal TotalIssues { get; init; }
    }
}
