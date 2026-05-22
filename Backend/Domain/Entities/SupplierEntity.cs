namespace Domain.Entities
{
    public class SupplierEntity : BaseAuditEntity
    {
        public string? CompanyName { get; set; }
        public string? Contact { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<GoodsReceiptEntity> GoodsReceipt { get; set; } = new List<GoodsReceiptEntity>();
    }
}
