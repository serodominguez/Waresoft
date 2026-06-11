namespace Infrastructure.Persistences.ReadModels.Dashboard
{
    public record DashboardTransferByStoreReadModel
    {
        public string StoreName { get; init; } = null!;
        public int TotalTransfers { get; init; }
    }
}
