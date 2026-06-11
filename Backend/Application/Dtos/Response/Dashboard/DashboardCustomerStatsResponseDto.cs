namespace Application.Dtos.Response.Dashboard
{
    public record DashboardCustomerStatsResponseDto
    {
        public int TotalActive { get; init; }
        public decimal PercentageChange { get; init; }
        public bool IsPositive { get; init; }
    }
}
