namespace Infrastructure.Persistences.ReadModels.Dashboard
{
    public record DashboardGoodsIssueStatsReadModel
    {
        public int TotalIssues { get; init; }
        public int IssuedLast7Days { get; init; }
        public int IssuedPrevious7Days { get; init; }
    }
}
