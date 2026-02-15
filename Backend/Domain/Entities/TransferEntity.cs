namespace Domain.Entities
{
    public class TransferEntity
    {
        public int IdTransfer { get; set; }
        public string? Code { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdStoreOrigin { get; set; }
        public int IdStoreDestination { get; set; }
        public int? AuditCreateUser { get; set; }
        public DateTime? AuditCreateDate { get; set; }
        public int? AuditUpdateUser { get; set; }
        public DateTime? AuditUpdateDate { get; set; }
        public int? AuditDeleteUser { get; set; }
        public DateTime? AuditDeleteDate { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public virtual StoreEntity StoreOrigin { get; set; } = null!;
        public virtual StoreEntity StoreDestination { get; set; } = null!;
        public ICollection<TransferDetailsEntity> TransferDetails { get; set; } = new List<TransferDetailsEntity>();
    }
}
