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
            var sequence = await _context.Sequence
                .FirstOrDefaultAsync(s => s.Name == "TRANSFER");

            if (sequence == null)
            {
                sequence = new SequenceEntity
                {
                    Name = "TRANSFER",
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

            return $"TRAS-{sequence.CurrentValue.ToString().PadLeft(6, '0')}";
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
                        StoreName = t.StoreOrigin.StoreName
                    },
                    IdStoreDestination = t.IdStoreDestination,
                    StoreDestination = new StoreEntity
                    {
                        Id = t.StoreDestination.Id,
                        StoreName = t.StoreDestination.StoreName
                    },
                    AuditCreateUser = t.AuditCreateUser,
                    AuditCreateDate = t.AuditCreateDate,
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
