namespace Application.Dtos.Response.Transfer
{
    public record TransferStatsResponseDto
    {
        // Pendientes
        public int TotalPending { get; init; }
        public int DifferenceVsYesterday { get; init; }
        public bool IsPendingPositive { get; init; }

        // Enviados
        public int TotalSent { get; init; }
        public decimal SentPercentageChange { get; init; }
        public bool IsSentPositive { get; init; }
    }
}
