namespace Infrastructure.Persistences.ReadModels.Transfer
{
    public record TransferStatsReadModel
    {
        // Pendientes (destino)
        public int TotalPending { get; init; }
        public int PendingToday { get; init; }
        public int PendingYesterday { get; init; }

        // Enviados (origen)
        public int TotalSent { get; init; }
        public int SentThisMonth { get; init; }
        public int SentLastMonth { get; init; }
    }
}
