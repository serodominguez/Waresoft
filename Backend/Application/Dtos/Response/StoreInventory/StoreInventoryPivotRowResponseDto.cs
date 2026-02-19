namespace Application.Dtos.Response.StoreInventory
{
    public class StoreInventoryPivotRowResponseDto
    {
        public string? Code { get; set; }
        public string? Color { get; set; }
        public string? BrandName { get; set; }
        public string? CategoryName { get; set; }
        public string? AuditCreateDate { get; set; }
        public Dictionary<string, int>? StockByStore { get; set; }
    }
}
