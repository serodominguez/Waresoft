namespace Application.Dtos.Response.Transfer
{
    public class TransferResponseDto
    {
        public int IdTransfer { get; set; }
        public string? Code { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdStoreOrigin { get; set; }
        public string? StoreOrigin { get; set; }
        public int IdStoreDestination { get; set; }
        public string? StoreDestination { get; set; }
        public int? AuditCreateUser { get; set; }
        public DateTime? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusTransfer { get; set; }
    }
}
