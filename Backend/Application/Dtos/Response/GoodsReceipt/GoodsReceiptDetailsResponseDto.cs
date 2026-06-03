namespace Application.Dtos.Response.GoodsReceipt
{
    public record GoodsReceiptDetailsResponseDto
    {
        public int Item { get; init; }
        public int IdProduct { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
        public string? Material { get; init; }
        public string? Color { get; init; }
        public string? CategoryName { get; init; }
        public string? BrandName { get; init; }
        public int Quantity { get; init; }
        public decimal UnitCost { get; init; }
        public decimal TotalCost { get; init; }
    }
}
