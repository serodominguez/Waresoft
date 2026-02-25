using DocumentFormat.OpenXml.Vml.Office;
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
                .Where(i => i.IdStore == storeId)
                .Select(i => new GoodsIssueEntity
                {
                    IdIssue = i.IdIssue,
                    Code = i.Code,
                    Type = i.Type,
                    TotalAmount = i.TotalAmount,
                    Annotations = i.Annotations,
                    IdUser = i.IdUser,
                    User = new UserEntity
                    {
                        Id = i.User.Id,
                        Names = i.User.Names,
                        LastNames = i.User.LastNames
                    },
                    IdStore = i.IdStore,
                    Store = new StoreEntity
                    {
                        Id = i.Store.Id,
                        StoreName = i.Store.StoreName
                    },
                    AuditCreateUser = i.AuditCreateUser,
                    AuditCreateDate = i.AuditCreateDate,
                    Status = i.Status
                });
        }

        public IQueryable<GoodsIssueEntity> GetGoodsIssueByIdAsQueryable(int issueId)
        {
            return _context.GoodsIssue
                .Where(i => i.IdIssue == issueId)
                .Include(i => i.User)
                .Include(i => i.Store);
        }

        public async Task AddGoodsIssueAsync(GoodsIssueEntity entity)
        {
            await _context.AddAsync(entity);
        }
    }
}
