namespace Application.Dtos.Request.GoodsReceipt
{
    public class GoodsReceiptRequestDto
    {
        public string? Type { get; set; }
        public string? DocumentDate { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdSupplier { get; set; }
        public int IdStore { get; set; }
        public ICollection<GoodsReceiptDetailsRequestDto> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetailsRequestDto>();
    }
}
