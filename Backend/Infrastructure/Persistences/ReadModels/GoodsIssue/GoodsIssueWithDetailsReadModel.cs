namespace Infrastructure.Persistences.ReadModels.GoodsIssue
{
    public record GoodsIssueWithDetailsReadModel
    {
        public int Id { get; init; }
        public string? Code { get; init; }
        public string? Type { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Annotations { get; init; }
        public int? IdUser { get; init; }
        public string? Names { get; init; }
        public string? LastNames { get; init; }
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public int? AuditCreateUser { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public int Status { get; init; }

        public ICollection<GoodsIssueDetailsReadModel> GoodsIssueDetails { get; init; } = new List<GoodsIssueDetailsReadModel>();
    }
}
