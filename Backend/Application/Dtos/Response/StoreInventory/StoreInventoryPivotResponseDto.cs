namespace Application.Dtos.Response.StoreInventory
{
    public record StoreInventoryPivotResponseDto
    {
        public List<string> Stores { get; init; } = new();
        public List<StoreInventoryPivotRowResponseDto> Rows { get; init; } = new();
    }
}
