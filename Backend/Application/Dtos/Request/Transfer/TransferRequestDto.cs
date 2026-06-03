using Domain.Entities;

namespace Application.Dtos.Request.Transfer
{
    public record TransferRequestDto
    {
        public decimal TotalAmount { get; init; }
        public string? Annotations { get; init; }
        public int IdStoreOrigin { get; init; }
        public int IdStoreDestination { get; init; }
        public ICollection<TransferDetailsRequestDto> TransferDetails { get; init; } = new List<TransferDetailsRequestDto>();
    }
}
