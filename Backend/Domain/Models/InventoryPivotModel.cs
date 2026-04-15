namespace Domain.Models
{
    public record InventoryPivotModel
    {
        public string? Image { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
        public string? Material { get; init; }
        public string? Color { get; init; }
        public string? BrandName { get; init; }
        public string? CategoryName { get; init; }
        public DateTime AuditCreateDate { get; init; }
        public string? StoreStocks { get; init; }
    }
}
