using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Transfer;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Transfer;

namespace Infrastructure.Persistences.Repositories.Transfer
{
    public class TransferDetailsQueryRepository : ITransferDetailsQueryRepository
    {
        private readonly DbContextSystem _context;

        public TransferDetailsQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<TransferDetailsReadModel> GetTransferDetailsQueryable(int transferId)
        {
            return _context.TransferDetails
                .Where(d => d.IdTransfer == transferId)
                .OrderBy(d => d.Item)
                .Select(TransferProjection.ToDetails);
        }
    }
}
