using Domain.Entities;

namespace Application.Dtos.Response.GoodsReceipt
{
    public class GoodsReceiptResponseDto
    {
        public int IdReceipt { get; set; }
        public string? Code { get; set; }
        public string? Type { get; set; }
        public string? DocumentDate { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdSupplier { get; set; }
        public string? CompanyName { get; set; }
        public int IdStore { get; set; }
        public string? StoreName { get; set; }
        public int? AuditCreateUser { get; set; }
        public string? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusReceipt { get; set; }
    }
}
