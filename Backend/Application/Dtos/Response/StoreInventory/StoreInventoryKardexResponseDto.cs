namespace Application.Dtos.Response.StoreInventory
{
    public class StoreInventoryKardexResponseDto
    {
        public string? ProductDescription { get; set; }
        public string? ProductCode { get; set; }
        public int CurrentStock { get; set; }
        public List<StoreInventoryKardexMovementDto> Movements { get; set; } = new();
    }
}
