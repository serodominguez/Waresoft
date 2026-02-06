using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GoodsIssueRepository : IGoodsIssueRepository
    {
        private readonly DbContextSystem _context;

        public GoodsIssueRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<string> GenerateCodeAsync()
        {
            var sequence = await _context.Sequence
                .FirstOrDefaultAsync(s => s.Name == "GOODS_ISSUE");

            if (sequence == null)
            {
                sequence = new SequenceEntity
                {
                    Name = "GOODS_ISSUE",
                    CurrentValue = 1,
                    LastUpdated = DateTime.Now
                };
                await _context.Sequence.AddAsync(sequence);
            }
            else
            {
                sequence.CurrentValue++;
                sequence.LastUpdated = DateTime.Now;
                _context.Sequence.Update(sequence);
            }

            await _context.SaveChangesAsync();

            return $"SAL-{sequence.CurrentValue.ToString().PadLeft(6, '0')}";
        }

        public IQueryable<GoodsIssueEntity> GetGoodsIssueQueryableByStore(int storeId)
        {
            return _context.GoodsIssue
                .AsNoTracking()
                .Where(i => i.IdStore == storeId)
                .Include(i => i.User)
                .Include(i => i.Store);
        }

        public async Task<GoodsIssueEntity?> GetGoodsIssueByIdAsync(int issueId)
        {
            var entity = await _context.GoodsIssue
                .AsNoTracking()
                .Include(i => i.User)
                .Include(i => i.Store)
                .FirstOrDefaultAsync(x => x.IdIssue.Equals(issueId));

            return entity;
        }

        public async Task<bool> RegisterGoodsIssueAsync(GoodsIssueEntity entity)
        {
            await _context.AddAsync(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> CancelGoodsIssueAsync(GoodsIssueEntity entity)
        {
            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
