namespace Application.Dtos.Response.Dashboard
{
    public record DashboardProductReplenishmentResponseDto
    {
        public int Available { get; init; }
        public int NotAvailable { get; init; }
        public int Discontinued { get; init; }
    }
}
