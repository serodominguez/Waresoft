using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Persistences.Repositories
{
    public class TransferDetailsRepository : ITransferDetailsRepository
    {
        private readonly DbContextSystem _context;

        public TransferDetailsRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransferDetailsEntity>> GetTransferDetailsAsync(int transferId)
        {
            return await _context.TransferDetails
                .AsNoTracking()
                .Include(d => d.Product)
                    .ThenInclude(p => p.Brand)
                .Include(d => d.Product)
                    .ThenInclude(p => p.Category)
                .Where(d => d.IdTransfer == transferId)
                .OrderBy(d => d.Item)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransferDetailsEntity>> GetTransferDetailsByProductAsync(int storeId, int productId)
        {
            return await _context.TransferDetails
                .AsNoTracking()
                .Include(d => d.Transfer)
                .Where(d => d.IdProduct == productId && (d.Transfer.IdStoreOrigin == storeId || d.Transfer.IdStoreDestination == storeId) && d.Transfer!.IsActive)
                .ToListAsync();
        }
    }
}
