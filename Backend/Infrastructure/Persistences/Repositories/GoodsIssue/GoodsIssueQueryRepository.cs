using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.GoodsIssue;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.GoodsIssue;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.GoodsIssue
{
    public class GoodsIssueQueryRepository : IGoodsIssueQueryRepository
    {
        private readonly DbContextSystem _context;

        public GoodsIssueQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<GoodsIssueReadModel> GetGoodsIssueQueryableByStore(int storeId)
        {
            return _context.GoodsIssue
                .AsNoTracking()
                .Where(i => i.IdStore == storeId)
                .Select(GoodsIssueProjection.ToSummary);
        }

        public IQueryable<GoodsIssueWithDetailsReadModel> GetGoodsIssueByIdAsQueryable(int issueId)
        {
            return _context.GoodsIssue
                .AsNoTracking()
                .Where(i => i.Id == issueId)
                .Select(GoodsIssueProjection.ToWithDetails);
        }
    }
}
