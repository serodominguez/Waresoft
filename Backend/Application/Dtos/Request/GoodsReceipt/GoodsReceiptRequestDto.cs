namespace Application.Dtos.Request.GoodsReceipt
{
    public record GoodsReceiptRequestDto
    {
        public string? Type { get; init; }
        public string? DocumentDate { get; init; }
        public string? DocumentType { get; init; }
        public string? DocumentNumber { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Annotations { get; init; }
        public int? IdSupplier { get; init; }
        public int IdStore { get; init; }
        public ICollection<GoodsReceiptDetailsRequestDto> GoodsReceiptDetails { get; init; } = new List<GoodsReceiptDetailsRequestDto>();
    }
}
