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
                .AsNoTracking()
                .Where(t => t.IdStoreOrigin == storeId || t.IdStoreDestination == storeId)
                .Include(t => t.StoreOrigin)
                .Include(t => t.StoreDestination);
        }

        public async Task<TransferEntity?> GetTransferByIdAsync(int transferId)
        {
            var entity = await _context.Transfer
                .AsNoTracking()
                .Include(t => t.StoreOrigin)
                .Include(t => t.StoreDestination)
                .FirstOrDefaultAsync(x => x.IdTransfer.Equals(transferId));

            return entity;
        }

        public async Task<bool> SendTransferAsync(TransferEntity entity)
        {
            await _context.AddAsync(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> ReceiveTransferAsync(TransferEntity entity)
        {
            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> CancelTransferAsync(TransferEntity entity)
        {
            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
