namespace Application.Dtos.Response.StoreInventory
{
    public class StoreInventoryPivotRowResponseDto
    {
        public string? Codigo { get; set; }
        public string? Color { get; set; }
        public string? Marca { get; set; }
        public string? Categoria { get; set; }
        public Dictionary<string, int>? StockByStore { get; set; }
    }
}
