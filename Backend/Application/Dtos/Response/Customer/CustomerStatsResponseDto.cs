namespace Application.Dtos.Response.Customer
{
    public record CustomerStatsResponseDto
    {
        public int TotalActive { get; init; }
        public decimal PercentageChange { get; init; }
        public bool IsPositive { get; init; }
    }
}
