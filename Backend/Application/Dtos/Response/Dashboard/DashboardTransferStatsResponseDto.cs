namespace Application.Dtos.Response.Dashboard
{
    public record DashboardTransferStatsResponseDto
    {
        public int TotalPending { get; init; }
        public int DifferenceVsYesterday { get; init; }
        public bool IsPendingPositive { get; init; }
    }
}
