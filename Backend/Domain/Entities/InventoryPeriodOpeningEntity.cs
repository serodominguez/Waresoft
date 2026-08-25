namespace Domain.Entities
{
    public class InventoryPeriodOpeningEntity
    {
        public int IdPeriod { get; set; }
        public int IdProduct { get; set; }
        public int OpeningStock { get; set; }

        public virtual InventoryPeriodEntity InventoryPeriod { get; set; } = null!;
        public virtual ProductEntity Product { get; set; } = null!;
    }
}
