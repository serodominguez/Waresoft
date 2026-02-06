namespace Application.Dtos.Response.GoodsReceipt
{
    public class GoodsReceiptDetailsResponseDto
    {
        public int Item { get; set; }
        public int IdProduct { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
