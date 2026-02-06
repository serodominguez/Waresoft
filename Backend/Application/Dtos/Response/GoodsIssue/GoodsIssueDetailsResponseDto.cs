namespace Application.Dtos.Response.GoodsIssue
{
    public class GoodsIssueDetailsResponseDto
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
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
