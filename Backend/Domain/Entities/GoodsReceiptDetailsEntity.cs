namespace Domain.Entities
{
    public class GoodsReceiptDetailsEntity
    {
        public int IdReceipt { get; set; }
        public int Item { get; set; }
        public int IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public virtual GoodsReceiptEntity GoodsReceipt { get; set; } = null!;
        public virtual ProductEntity Product { get; set; } = null!;
    }
}
