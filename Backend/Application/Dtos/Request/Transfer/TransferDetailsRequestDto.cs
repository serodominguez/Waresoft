namespace Application.Dtos.Request.Transfer
{
    public class TransferDetailsRequestDto
    {
        public int Item { get; set; }
        public int IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
