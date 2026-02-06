namespace Domain.Entities
{
    public class ProductEntity : BaseEntity
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }
        public string? UnitMeasure { get; set; }
        public int IdBrand { get; set; }
        public int IdCategory { get; set; }
        public virtual BrandEntity Brand { get; set; } = null!;
        public virtual CategoryEntity Category { get; set; } = null!;
        public virtual ICollection<StoreInventoryEntity> Inventory { get; set; } = new List<StoreInventoryEntity>();
        public virtual ICollection<GoodsIssueDetailsEntity> GoodsIssueDetails { get; set; } = new List<GoodsIssueDetailsEntity>();
        public virtual ICollection<GoodsReceiptDetailsEntity> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetailsEntity>();
        public virtual ICollection<TransferDetailsEntity> TransferDetails { get; set; } = new List<TransferDetailsEntity>();
    }
}
