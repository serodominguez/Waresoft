namespace Application.Dtos.Request.GoodsIssue
{
    public record GoodsIssueRequestDto
    {
        public string? Type { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Annotations { get; init; }
        public int? IdUser { get; init; }
        public int IdStore { get; init; }

        public ICollection<GoodsIssueDetailsRequestDto> GoodsIssueDetails { get; init; } = new List<GoodsIssueDetailsRequestDto>();
    }
}
