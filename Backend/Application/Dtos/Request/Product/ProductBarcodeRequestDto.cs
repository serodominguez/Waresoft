namespace Application.Dtos.Request.Product
{
    public record ProductBarcodeRequestDto
    {
        public int IdProduct { get; init; }
        public int Quantity { get; init; }
    }
}
