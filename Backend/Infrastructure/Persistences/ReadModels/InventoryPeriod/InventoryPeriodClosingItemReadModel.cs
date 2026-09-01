namespace Infrastructure.Persistences.ReadModels.InventoryPeriod
{
    public record InventoryPeriodClosingItemReadModel
    {
        public int IdPeriod { get; init; }
        public int IdProduct { get; init; }
        public string? ProductCode { get; init; }
        public string? ProductDescription { get; init; }
        public string? UnitMeasure { get; init; }
        public int SystemStock { get; init; }
        public int? PhysicalStock { get; init; }
        public int? Difference { get; init; }
        public int ClosingStock { get; init; }
    }
}
