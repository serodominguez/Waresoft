namespace Application.Dtos.Response.StoreInventory
{
    public record StoreInventoryPivotRowResponseDto
    {
        public string? Image { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
        public string? Material { get; init; }
        public string? Color { get; init; }
        public string? BrandName { get; init; }
        public string? CategoryName { get; init; }
        public string? AuditCreateDate { get; init; }
        public Dictionary<string, int>? StockByStore { get; init; }
    }
}
