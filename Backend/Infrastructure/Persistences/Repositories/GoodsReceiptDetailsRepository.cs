using DocumentFormat.OpenXml.Bibliography;
using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GoodsReceiptDetailsRepository : IGoodsReceiptDetailsRepository
    {
        private readonly DbContextSystem _context;

        public GoodsReceiptDetailsRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GoodsReceiptDetailsEntity>> GetGoodsReceiptDetailsAsync(int receiptId)
        {
            return await _context.GoodsReceiptDetails
                .AsNoTracking()
                .Include(d => d.Product)
                    .ThenInclude(p => p.Brand)
                .Include(d => d.Product)
                    .ThenInclude(p => p.Category)
                .Where(d => d.IdReceipt == receiptId)
                .OrderBy(d => d.Item)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceiptDetailsEntity>> GetGoodsReceiptDetailsByProductAsync(int storeId, int productId)
        {
            return await _context.GoodsReceiptDetails
                .AsNoTracking()
                .Include(d => d.GoodsReceipt)
                .Where(d => d.IdProduct == productId && d.GoodsReceipt.IdStore == storeId && d.GoodsReceipt!.IsActive)
                .ToListAsync();
        }
    }
}
