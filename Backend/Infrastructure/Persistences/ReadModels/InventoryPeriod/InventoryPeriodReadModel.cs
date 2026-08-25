namespace Infrastructure.Persistences.ReadModels.InventoryPeriod
{
    public record InventoryPeriodReadModel
    {
        public int IdPeriod { get; init; }
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public string? PeriodName { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public string? Status { get; init; }
        public int OpenedByUser { get; init; }
        public DateTime OpenedDate { get; init; }
        public int? ClosedByUser { get; init; }
        public DateTime? ClosedDate { get; init; }
    }
}
