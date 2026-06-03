namespace Application.Dtos.Response.GoodsIssue
{
    public record GoodsIssueResponseDto
    {
        public int IdIssue { get; init; }
        public string? Code { get; init; }
        public string? Type { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Annotations { get; init; }
        public int? IdUser { get; init; }
        public string? UserName { get; init; }
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public int? AuditCreateUser { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusIssue { get; init; }
    }
}
