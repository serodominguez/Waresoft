namespace Application.Dtos.Response.Dashboard
{
    public record DashboardGoodsIssueStatsResponseDto
    {
        public int TotalIssues { get; init; }
        public int DifferenceVsLast7Days { get; init; }
        public bool IsPositive { get; init; }
    }
}
