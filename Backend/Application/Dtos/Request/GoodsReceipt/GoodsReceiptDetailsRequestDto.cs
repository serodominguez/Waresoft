namespace Application.Dtos.Request.GoodsReceipt
{
    public record GoodsReceiptDetailsRequestDto
    {
        public int Item { get; init; }
        public int IdProduct { get; init; }
        public int Quantity { get; init; }
        public decimal UnitCost { get; init; }
        public decimal TotalCost { get; init; }
    }
}
