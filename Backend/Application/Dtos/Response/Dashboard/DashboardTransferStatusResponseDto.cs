namespace Application.Dtos.Response.Dashboard
{
    public record DashboardTransferStatusResponseDto
    {
        public string Month { get; init; } = null!;
        public int Sent { get; init; }
        public int Pending { get; init; }
        public int Received { get; init; }
    }
}
