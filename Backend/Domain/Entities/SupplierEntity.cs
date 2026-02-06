namespace Domain.Entities
{
    public class SupplierEntity : BaseEntity
    {
        public string? CompanyName { get; set; }
        public string? Contact { get; set; }
        public int? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public virtual ICollection<GoodsReceiptEntity> GoodsReceipt { get; set; } = new List<GoodsReceiptEntity>();
    }
}
