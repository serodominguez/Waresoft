namespace Domain.Entities
{
    public class InventoryPeriodClosingEntity
    {
        public int IdPeriod { get; set; }
        public int IdProduct { get; set; }
        public int SystemStock { get; set; }
        public int? PhysicalStock { get; set; }
        public int? Difference { get; set; }
        public int ClosingStock { get; set; }

        public virtual InventoryPeriodEntity InventoryPeriod { get; set; } = null!;
        public virtual ProductEntity Product { get; set; } = null!;
    }
}
