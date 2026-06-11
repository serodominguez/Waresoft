namespace Application.Dtos.Response.Dashboard
{
    public record DashboardStoreInventoryStatsResponseDto
    {
        public int BelowMinimum { get; init; }
        public int DifferenceVsLastMonth { get; init; }
        public bool IsPositive { get; init; }
    }
}
