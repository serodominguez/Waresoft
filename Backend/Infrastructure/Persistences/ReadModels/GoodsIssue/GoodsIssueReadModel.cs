namespace Infrastructure.Persistences.ReadModels.GoodsIssue
{
    public record GoodsIssueReadModel
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
    }
}
