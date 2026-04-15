namespace Application.Dtos.Response.StoreInventory
{
    public class StoreInventoryKardexResponseDto
    {
        public int IdProduct { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }
        public string? UnitMeasure { get; set; }
        public int CurrentStock { get; set; }
        public int CalculatedStock { get; set; }
        public int StockDifference { get; set; }
        public List<StoreInventoryKardexMovementDto> Movements { get; set; } = new();
    }
}
