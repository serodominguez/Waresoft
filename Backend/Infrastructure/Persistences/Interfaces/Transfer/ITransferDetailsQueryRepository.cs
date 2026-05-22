using Infrastructure.Persistences.ReadModels.Transfer;

namespace Infrastructure.Persistences.Interfaces.Transfer
{
    public interface ITransferDetailsQueryRepository
    {
        IQueryable<TransferDetailsReadModel> GetTransferDetailsQueryable(int transferId);
    }
}
