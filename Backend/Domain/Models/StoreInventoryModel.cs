namespace Domain.Models
{
    public record StoreInventoryModel
    {
        public int IdStore { get; init; }
        public int IdProduct { get; init; }
        public int StockAvailable { get; init; }
        public int CalculatedStock { get; init; }
        public int StockInTransit { get; init; }
        public decimal Price { get; init; }
        public int? Replenishment { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
        public string? Material { get; init; }
        public string? Color { get; init; }
        public string? UnitMeasure { get; init; }
        public string? BrandName { get; init; }
        public string? CategoryName { get; init; }
        public DateTime AuditCreateDate { get; init; }
    }
}
