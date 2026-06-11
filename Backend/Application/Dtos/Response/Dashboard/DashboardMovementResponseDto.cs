namespace Application.Dtos.Response.Dashboard
{
    public record DashboardMovementResponseDto
    {
        public string Month { get; init; } = null!;
        public decimal Receipts { get; init; }
        public decimal Issues { get; init; }
    }
}
