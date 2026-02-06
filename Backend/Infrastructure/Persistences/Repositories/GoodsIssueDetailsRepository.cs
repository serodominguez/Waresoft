using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GoodsIssueDetailsRepository : IGoodsIssueDetailsRepository
    {
        private readonly DbContextSystem _context;

        public GoodsIssueDetailsRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GoodsIssueDetailsEntity>> GetGoodsIssueDetailsAsync(int issueId)
        {
            return await _context.GoodsIssueDetails
                .AsNoTracking()
                .Include(d => d.Product)
                    .ThenInclude(p => p.Brand)
                .Include(d => d.Product)
                    .ThenInclude(p => p.Category)
                .Where(d => d.IdIssue == issueId)
                .OrderBy(d => d.Item)
                .ToListAsync();
        }
    }
}
