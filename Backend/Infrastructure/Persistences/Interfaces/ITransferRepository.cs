using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ITransferRepository
    {
        Task<string> GenerateCodeAsync();
        IQueryable<TransferEntity> GetTransferQueryableByStore(int storeId);
        IQueryable<TransferEntity> GetTransferByIdAsQueryable(int transferId);
        Task AddTransferAsync(TransferEntity entity);

    }
}
