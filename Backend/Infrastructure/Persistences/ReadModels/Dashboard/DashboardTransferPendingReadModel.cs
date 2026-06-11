namespace Infrastructure.Persistences.ReadModels.Dashboard
{
    public record DashboardTransferPendingReadModel
    {
        public int TotalPending { get; init; }
        public int PendingToday { get; init; }
        public int PendingYesterday { get; init; }

    }
}
