namespace Infrastructure.Persistences.ReadModels.GoodsReceipt
{
    public record GoodsReceiptWithDetailsReadModel
    {
        public int Id { get; init; }
        public string? Code { get; init; }
        public DateTime? DocumentDate { get; init; }
        public string? Type { get; init; }
        public string? DocumentType { get; init; }
        public string? DocumentNumber { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Annotations { get; init; }
        public int? IdSupplier { get; init; }
        public string? CompanyName { get; init; }
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public int? AuditCreateUser { get; init; }
        public string? AuditCreateName { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public int Status { get; init; }

        public ICollection<GoodsReceiptDetailsReadModel> GoodsReceiptDetails { get; init; } = new List<GoodsReceiptDetailsReadModel>();
    }
}
