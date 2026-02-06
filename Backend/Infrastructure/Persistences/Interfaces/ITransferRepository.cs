using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ITransferRepository
    {
        Task<string> GenerateCodeAsync();
        IQueryable<TransferEntity> GetTransferQueryableByStore(int storeId);
        Task<TransferEntity?> GetTransferByIdAsync(int transferId);
        Task<bool> SendTransferAsync(TransferEntity entity);
        Task<bool> ReceiveTransferAsync(TransferEntity entity);
        Task<bool> CancelTransferAsync(TransferEntity entity);

    }
}
