namespace Application.Dtos.Response.Dashboard
{
    public record DashboardTransferByStoreResponseDto
    {
        public string StoreName { get; init; } = null!;
        public int TotalTransfers { get; init; }
    }
}
