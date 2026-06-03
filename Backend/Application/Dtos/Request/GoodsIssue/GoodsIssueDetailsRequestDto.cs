namespace Application.Dtos.Request.GoodsIssue
{
    public record GoodsIssueDetailsRequestDto
    {
        public int Item { get; init; }
        public int IdProduct { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
    }
}
