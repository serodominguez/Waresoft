using Infrastructure.Persistences.ReadModels.Transfer;

namespace Infrastructure.Persistences.Interfaces.Transfer
{
    public interface ITransferQueryRepository
    {
        IQueryable<TransferReadModel> GetTransferQueryableByStore(int storeId);
        IQueryable<TransferWithDetailsReadModel> GetTransferByIdAsQueryable(int transferId);
        Task<TransferStatsReadModel> GetTransferStatsAsync(int storeId, CancellationToken cancellationToken);
    }
}
