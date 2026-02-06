namespace Application.Dtos.Request.GoodsIssue
{
    public class GoodsIssueDetailsRequestDto
    {
        public int Item { get; set; }
        public int IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
