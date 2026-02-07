using Application.Dtos.Response.GoodsReceipt;

namespace Application.Dtos.Response.Transfer
{
    public class TransferWithDetailsResponseDto
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
        public int? AuditCreateUser { get; set; }
        public string? AuditCreateName { get; set; }
        public int? AuditUpdateUser { get; set; }
        public string? AuditUpdateName { get; set; }
        public string? StatusTransfer { get; set; }

        public ICollection<TransferDetailsResponseDto> TransferDetails { get; set; } = new List<TransferDetailsResponseDto>();
    }
}
