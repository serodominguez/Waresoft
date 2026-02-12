namespace Application.Dtos.Response.Transfer
{
    public class TransferResponseDto
    {
        public int IdTransfer { get; set; }
        public string? Code { get; set; }
        public string? SendDate { get; set; }
        public string? ReceiveDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdStoreOrigin { get; set; }
        public string? StoreOrigin { get; set; }
        public int IdStoreDestination { get; set; }
        public string? StoreDestination { get; set; }
        public string? SendUser { get; set; }
        public string? ReceiveUser { get; set; }
        public int Status { get; set; }
        public string? StatusTransfer { get; set; }
    }
}
