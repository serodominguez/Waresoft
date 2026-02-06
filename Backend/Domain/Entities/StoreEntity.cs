namespace Domain.Entities
{
    public class StoreEntity : BaseEntity
    {
        public string? StoreName { get; set; }
        public string? Manager { get; set; }
        public string? Address { get; set; }
        public int? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Email { get; set; }
        public string? Type { get; set; }
        public virtual ICollection<UserEntity> User { get; set; } = new List<UserEntity>();
        public virtual ICollection<StoreInventoryEntity> Inventory { get; set; } = new List<StoreInventoryEntity>();
        public virtual ICollection<GoodsIssueEntity> GoodsIssue { get; set; } = new List<GoodsIssueEntity>();
        public virtual ICollection<GoodsReceiptEntity> GoodsReceipt { get; set; } = new List<GoodsReceiptEntity>();
        public ICollection<TransferEntity> TransfersAsOrigin { get; set; } = new List<TransferEntity>();
        public ICollection<TransferEntity> TransfersAsDestination { get; set; } = new List<TransferEntity>();
    }
}
