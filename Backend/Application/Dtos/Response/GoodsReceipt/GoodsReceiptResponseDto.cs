namespace Application.Dtos.Response.GoodsReceipt
{
    public record GoodsReceiptResponseDto
    {
        public int IdReceipt { get; init; }
        public string? Code { get; init; }
        public string? Type { get; init; }
        public string? DocumentDate { get; init; }
        public string? DocumentType { get; init; }
        public string? DocumentNumber { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Annotations { get; init; }
        public int? IdSupplier { get; init; }
        public string? CompanyName { get; init; }
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public int? AuditCreateUser { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusReceipt { get; init; }
    }
}
