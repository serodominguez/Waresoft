namespace Infrastructure.Persistences.ReadModels.InventoryPeriod
{
    public record InventoryPeriodOpeningReadModel
    {
        public int IdPeriod { get; init; }
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public string? PeriodName { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public int Status { get; init; }
        public int OpenedByUser { get; init; }
        public DateTime? OpenedDate { get; init; }
        public int TotalProducts { get; init; }
        public int TotalOpeningStock { get; init; }

        public List<InventoryPeriodOpeningItemReadModel> Items { get; init; } = [];
    }
}
