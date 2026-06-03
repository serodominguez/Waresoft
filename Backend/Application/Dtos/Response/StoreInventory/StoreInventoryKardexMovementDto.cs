namespace Application.Dtos.Response.StoreInventory
{
    public record StoreInventoryKardexMovementDto
    {
        public int IdProduct { get; init; }
        public int Quantity { get; init; }
        public int IdMovement { get; init; }
        public string? Code { get; init; }
        public string? Date { get; init; }
        public string? MovementType { get; init; }
        public string? Type { get; init; }
        public string? State { get; init; }
        public int AccumulatedStock { get; init; }
    }
}
