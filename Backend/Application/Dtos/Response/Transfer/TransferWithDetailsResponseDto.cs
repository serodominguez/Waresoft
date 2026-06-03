namespace Application.Dtos.Response.Transfer
{
    public record TransferWithDetailsResponseDto
    {
        public int IdTransfer { get; init; }
        public string? Code { get; init; }
        public string? SendDate { get; init; }
        public string? ReceiveDate { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Annotations { get; init; }
        public int IdStoreOrigin { get; init; }
        public string? StoreOrigin { get; init; }
        public int IdStoreDestination { get; init; }
        public string? StoreDestination { get; init; }
        public int? AuditCreateUser { get; init; }
        public string? SendUser { get; init; }
        public int? AuditUpdateUser { get; init; }
        public string? ReceiveUser { get; init; }
        public string? StatusTransfer { get; init; }

        public ICollection<TransferDetailsResponseDto> TransferDetails { get; init; } = new List<TransferDetailsResponseDto>();
    }
}
