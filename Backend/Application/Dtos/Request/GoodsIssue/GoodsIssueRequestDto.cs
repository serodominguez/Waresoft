namespace Application.Dtos.Request.GoodsIssue
{
    public class GoodsIssueRequestDto
    {
        public string? Type { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdUser { get; set; }
        public int IdStore { get; set; }

        public ICollection<GoodsIssueDetailsRequestDto>? GoodsIssueDetails { get; set; } = new List<GoodsIssueDetailsRequestDto>();
    }
}
