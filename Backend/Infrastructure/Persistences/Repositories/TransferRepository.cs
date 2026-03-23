using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private readonly DbContextSystem _context;

        public TransferRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<string> GenerateCodeAsync()
        {
            const string sequenceName = "TRANSFER";

            var sequence = await _context.Sequence
                .FromSqlRaw(@"SELECT NAME, CURRENT_VALUE, LAST_UPDATED FROM SEQUENCES WITH (UPDLOCK, ROWLOCK, HOLDLOCK) WHERE NAME = {0}", sequenceName)
                .AsTracking()
                .FirstOrDefaultAsync();

            if (sequence == null)
            {
                sequence = new SequenceEntity
                {
                    Name = "TRANSFER",
                    CurrentValue = 1,
                    LastUpdated = DateTime.UtcNow
                };
                await _context.Sequence.AddAsync(sequence);
            }
            else
            {
                sequence.CurrentValue++;
                sequence.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return $"TRP-{sequence.CurrentValue.ToString().PadLeft(6, '0')}";
        }

        public IQueryable<TransferEntity> GetTransferQueryableByStore(int storeId)
        {
            return _context.Transfer
                .Where(t => t.IdStoreOrigin == storeId || t.IdStoreDestination == storeId)
                .Select(t => new TransferEntity
                {
                    IdTransfer = t.IdTransfer,
                    Code = t.Code,
                    SendDate = t.SendDate,
                    ReceiveDate = t.ReceiveDate,
                    TotalAmount = t.TotalAmount,
                    Annotations = t.Annotations,
                    IdStoreOrigin = t.IdStoreOrigin,
                    StoreOrigin = new StoreEntity
                    {
                        Id = t.StoreOrigin.Id,
                        Type = t.StoreOrigin.Type,
                        StoreName = t.StoreOrigin.StoreName
                    },
                    IdStoreDestination = t.IdStoreDestination,
                    StoreDestination = new StoreEntity
                    {
                        Id = t.StoreDestination.Id,
                        Type = t.StoreDestination.Type,
                        StoreName = t.StoreDestination.StoreName
                    },
                    AuditCreateUser = t.AuditCreateUser,
                    AuditCreateDate = t.AuditCreateDate,
                    AuditUpdateUser = t.AuditUpdateUser,
                    AuditUpdateDate = t.AuditUpdateDate,
                    Status = t.Status,
                });
        }

        public IQueryable<TransferEntity> GetTransferByIdAsQueryable(int transferId)
        {
            return _context.Transfer
                .Where(t => t.IdTransfer == transferId)
                .Include(t => t.StoreOrigin)
                .Include(t => t.StoreDestination);
        }

        public async Task AddTransferAsync(TransferEntity entity)
        {
            await _context.AddAsync(entity);
        }
    }
}
