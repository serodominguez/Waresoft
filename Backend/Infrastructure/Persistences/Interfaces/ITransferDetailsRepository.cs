using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ITransferDetailsRepository
    {
        Task<IEnumerable<TransferDetailsEntity>> GetTransferDetailsAsync(int transferId);
    }
}
