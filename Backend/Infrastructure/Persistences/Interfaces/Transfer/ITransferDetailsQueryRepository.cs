using Infrastructure.Persistences.ReadModels.Transfer;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ITransferDetailsQueryRepository
    {
        IQueryable<TransferDetailsReadModel> GetTransferDetailsQueryable(int transferId);
    }
}
