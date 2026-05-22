using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.GoodsIssue;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.GoodsIssue;

namespace Infrastructure.Persistences.Repositories.GoodsIssue
{
    public class GoodsIssueDetailsQueryRepository : IGoodsIssueDetailsQueryRepository
    {
        private readonly DbContextSystem _context;

        public GoodsIssueDetailsQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<GoodsIssueDetailsReadModel> GetGoodsIssueDetailsQueryable(int issueId)
        {
            return _context.GoodsIssueDetails
                    .Where(d => d.IdIssue == issueId)
                    .OrderBy(d => d.Item)
                    .Select(GoodsIssueProjection.ToDetails);
        }
    }
}
