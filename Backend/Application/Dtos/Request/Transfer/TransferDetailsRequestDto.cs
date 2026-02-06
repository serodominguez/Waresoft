namespace Application.Dtos.Request.Transfer
{
    public class TransferDetailsRequestDto
    {
        public int Item { get; set; }
        public int IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
