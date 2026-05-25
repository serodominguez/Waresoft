namespace Domain.Entities
{
    public class TransferEntity : BaseAuditEntity
    {
        public string? Code { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdStoreOrigin { get; set; }
        public int IdStoreDestination { get; set; }
        public int Status { get; set; }

        public virtual StoreEntity StoreOrigin { get; set; } = null!;
        public virtual StoreEntity StoreDestination { get; set; } = null!;
        public ICollection<TransferDetailsEntity> TransferDetails { get; set; } = new List<TransferDetailsEntity>();
    }
}
