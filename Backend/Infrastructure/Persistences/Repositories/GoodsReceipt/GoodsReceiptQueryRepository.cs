using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.GoodsReceipt;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.GoodsReceipt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.GoodsReceipt
{
    public class GoodsReceiptQueryRepository : IGoodsReceiptQueryRepository
    {
        private readonly DbContextSystem _context;

        public GoodsReceiptQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<GoodsReceiptReadModel> GetGoodsReceiptQueryableByStore(int storeId)
        {
            return _context.GoodsReceipt
                .Where(r => r.IdStore == storeId)
                .Select(GoodsReceiptProjection.ToSummary);
        }

        public IQueryable<GoodsReceiptWithDetailsReadModel> GetGoodsReceiptByIdAsQueryable(int receiptId)
        {
            return _context.GoodsReceipt
                 .AsNoTracking()
                 .Where(r => r.Id == receiptId)
                 .Select(GoodsReceiptProjection.ToWithDetails);
        }
    }
}
