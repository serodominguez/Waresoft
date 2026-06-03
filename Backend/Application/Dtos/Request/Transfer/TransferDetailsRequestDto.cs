namespace Application.Dtos.Request.Transfer
{
    public record TransferDetailsRequestDto
    {
        public int Item { get; init; }
        public int IdProduct { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
    }
}
