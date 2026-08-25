namespace Domain.Entities
{
    public class InventoryPeriodEntity
    {
        public int IdPeriod { get; set; }
        public int IdStore { get; set; }
        public string? PeriodName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int OpenedByUser { get; set; }
        public DateTime OpenedDate { get; set; }
        public int? ClosedByUser { get; set; }
        public DateTime? ClosedDate { get; set; }

        public virtual StoreEntity Store { get; set; } = null!;
        public virtual ICollection<InventoryPeriodClosingEntity> InventoryPeriodClosing { get; set; } = new List<InventoryPeriodClosingEntity>();
        public virtual ICollection<InventoryPeriodOpeningEntity> InventoryPeriodOpening { get; set; } = new List<InventoryPeriodOpeningEntity>();
        public virtual ICollection<GoodsReceiptEntity> GoodsReceipt { get; set; } = new List<GoodsReceiptEntity>();
        public virtual ICollection<GoodsIssueEntity> GoodsIssue { get; set; } = new List<GoodsIssueEntity>();
        public virtual ICollection<TransferEntity> Transfer { get; set; } = new List<TransferEntity>();
    }
}
