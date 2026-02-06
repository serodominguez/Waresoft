namespace Domain.Entities
{
    public class TransferDetailsEntity
    {
        public int IdTransfer { get; set; }
        public int Item { get; set; }
        public int IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual TransferEntity Transfer { get; set; } = null!;
        public virtual ProductEntity Product { get; set; } = null!;
    }
}
