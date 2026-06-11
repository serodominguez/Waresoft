namespace Application.Dtos.Response.Dashboard
{
    public record DashboardProductStatsResponseDto
    {
        public int TotalActive { get; init; }
        public int NewThisMonth { get; init; }
    }
}
