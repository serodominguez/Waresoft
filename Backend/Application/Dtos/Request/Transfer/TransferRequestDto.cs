using Domain.Entities;

namespace Application.Dtos.Request.Transfer
{
    public class TransferRequestDto
    {
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdStoreOrigin { get; set; }
        public int IdStoreDestination { get; set; }
        public ICollection<TransferDetailsRequestDto> TransferDetails { get; set; } = new List<TransferDetailsRequestDto>();
    }
}
