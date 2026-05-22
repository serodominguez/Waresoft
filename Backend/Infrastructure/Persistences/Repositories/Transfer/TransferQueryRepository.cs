using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Transfer;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Transfer;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.Transfer
{
    public class TransferQueryRepository : ITransferQueryRepository
    {
        private readonly DbContextSystem _context;

        public TransferQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<TransferReadModel> GetTransferQueryableByStore(int storeId)
        {
            return _context.Transfer
                .Where(t => t.IdStoreOrigin == storeId || t.IdStoreDestination == storeId)
                .Select(TransferProjection.ToSummary);
        }

        public IQueryable<TransferWithDetailsReadModel> GetTransferByIdAsQueryable(int transferId)
        {
            return _context.Transfer
                 .AsNoTracking()
                 .Where(t => t.Id == transferId)
                 .Select(TransferProjection.ToWithDetails);
        }
    }
}
