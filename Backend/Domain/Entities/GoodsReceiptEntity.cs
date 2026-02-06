namespace Domain.Entities
{
    public class GoodsReceiptEntity
    {
        public int IdReceipt { get; set; }
        public string? Code { get; set; }
        public string? Type { get; set; }
        public DateTime DocumentDate { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdSupplier { get; set; }
        public int IdStore { get; set; }
        public int? AuditCreateUser { get; set; }
        public DateTime? AuditCreateDate { get; set; }
        public int? AuditDeleteUser { get; set; }
        public DateTime? AuditDeleteDate { get; set; }
        public bool Status { get; set; }
        public virtual StoreEntity Store { get; set; } = null!;
        public virtual SupplierEntity Supplier { get; set; } = null!;
        public virtual ICollection<GoodsReceiptDetailsEntity> GoodsReceiptDetails { get; set; } = null!;
    }
}
