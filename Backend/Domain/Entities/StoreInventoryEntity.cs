namespace Domain.Entities
{
    public class StoreInventoryEntity
    {
        public int IdStore { get; set; }
        public int IdProduct { get; set; }
        public int StockAvailable { get; set; }
        public int StockInTransit { get; set; }
        public decimal Price { get; set; }
        public int? AuditUpdateUser { get; set; }
        public DateTime? AuditUpdateDate { get; set; }
        public virtual ProductEntity Product { get; set; } = null!;
        public virtual StoreEntity Store { get; set; } = null!;
    }
}
