using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ITransferDetailsRepository
    {
        IQueryable<TransferDetailsEntity> GetTransferDetailsQueryable(int transferId);
    }
}
