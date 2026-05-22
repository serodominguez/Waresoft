using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.GoodsReceipt;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.GoodsReceipt;

namespace Infrastructure.Persistences.Repositories.GoodsReceipt
{
    public class GoodsReceiptDetailsQueryRepository : IGoodsReceiptDetailsQueryRepository
    {
        private readonly DbContextSystem _context;

        public GoodsReceiptDetailsQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<GoodsReceiptDetailsReadModel> GetGoodsReceiptDetailsQueryable(int receiptId)
        {
            return _context.GoodsReceiptDetails
                .Where(d => d.IdReceipt == receiptId)
                .OrderBy(d => d.Item)
                .Select(GoodsReceiptProjection.ToDetails);
        }
    }
}
