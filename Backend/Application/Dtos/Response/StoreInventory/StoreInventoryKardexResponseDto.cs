namespace Application.Dtos.Response.StoreInventory
{
    public record StoreInventoryKardexResponseDto
    {
        public int IdProduct { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
        public string? Material { get; init; }
        public string? Color { get; init; }
        public string? UnitMeasure { get; init; }
        public int CurrentStock { get; init; }
        public int CalculatedStock { get; init; }
        public int StockDifference { get; init; }
        public List<StoreInventoryKardexMovementDto> Movements { get; init; } = new();
    }
}
