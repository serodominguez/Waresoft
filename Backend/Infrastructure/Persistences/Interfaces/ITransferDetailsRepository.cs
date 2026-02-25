using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ITransferDetailsRepository
    {
        Task<IEnumerable<TransferDetailsEntity>> GetTransferDetailsAsync(int transferId);
        Task<IEnumerable<TransferDetailsEntity>> GetTransferDetailsByProductAsync(int storeId, int productId);
    }
}
