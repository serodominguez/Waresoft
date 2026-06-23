using Infrastructure.Persistences.ReadModels.Transfer;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ITransferQueryRepository
    {
        IQueryable<TransferReadModel> GetTransferQueryableByStore(int storeId);
        IQueryable<TransferWithDetailsReadModel> GetTransferByIdAsQueryable(int transferId);
    }
}
