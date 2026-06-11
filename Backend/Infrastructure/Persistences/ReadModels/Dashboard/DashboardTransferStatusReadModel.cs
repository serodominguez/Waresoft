namespace Infrastructure.Persistences.ReadModels.Dashboard
{
    public record DashboardTransferStatusReadModel
    {
        public int Month { get; init; }
        public int Year { get; init; }
        public int Sent { get; init; }
        public int Pending { get; init; }
        public int Received { get; init; }
    }
}
