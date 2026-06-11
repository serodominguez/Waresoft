namespace Application.Dtos.Request.StoreInventory
{
    public record StoreInventoryRequestDto
    {
        public int IdProduct { get; init; }
        public decimal Price { get; init; }
        public int MinimumStock { get; init; }
    }
}
