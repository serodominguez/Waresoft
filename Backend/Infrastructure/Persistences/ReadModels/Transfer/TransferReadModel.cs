namespace Infrastructure.Persistences.ReadModels.Transfer
{
    public record TransferReadModel
    {
        public int Id { get; init; }
        public string? Code { get; init; }
        public DateTime? SendDate { get; init; }
        public DateTime? ReceiveDate { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Annotations { get; init; }
        public int IdStoreOrigin { get; init; }
        public string? StoreOrigin { get; init; }
        public string? TypeOrigin { get; init; }
        public int IdStoreDestination { get; init; }
        public string? StoreDestination { get; init; }
        public string? TypeDestination { get; init; }
        public int? AuditCreateUser { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public int? AuditUpdateUser { get; init; }
        public DateTime? AuditUpdateDate { get; init; }
        public int Status { get; init; }
    }
}
