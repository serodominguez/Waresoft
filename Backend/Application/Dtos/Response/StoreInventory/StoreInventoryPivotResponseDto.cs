namespace Application.Dtos.Response.StoreInventory
{
    public class StoreInventoryPivotResponseDto
    {
        public List<string>? Stores { get; set; }
        public List<StoreInventoryPivotRowResponseDto>? Rows { get; set; }
    }
}
