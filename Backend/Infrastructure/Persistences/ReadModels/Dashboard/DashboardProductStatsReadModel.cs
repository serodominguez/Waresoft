namespace Infrastructure.Persistences.ReadModels.Dashboard
{
    public record DashboardProductStatsReadModel
    {
        public int TotalActive { get; init; }
        public int NewThisMonth { get; init; }
    }
}
