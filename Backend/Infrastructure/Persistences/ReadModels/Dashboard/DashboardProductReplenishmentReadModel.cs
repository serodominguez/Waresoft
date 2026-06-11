namespace Infrastructure.Persistences.ReadModels.Dashboard
{
    public record DashboardProductReplenishmentReadModel
    {
        public int Available { get; init; }
        public int NotAvailable { get; init; }
        public int Discontinued { get; init; }
    }
}
