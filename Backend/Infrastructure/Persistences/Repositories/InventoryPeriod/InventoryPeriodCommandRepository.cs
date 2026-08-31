using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.InventoryPeriod;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.InventoryPeriod
{
    public class InventoryPeriodCommandRepository : IInventoryPeriodCommandRepository
    {
        private readonly DbContextSystem _context;

        public InventoryPeriodCommandRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<InventoryPeriodEntity> OpenPeriodAsync(InventoryPeriodEntity period)
        {
            await _context.InventoryPeriod.AddAsync(period);
            await _context.SaveChangesAsync();
            return period;
        }

        public async Task SaveClosingAsync(List<InventoryPeriodClosingEntity> closing)
        {
            await _context.InventoryPeriodClosing.AddRangeAsync(closing);
            await _context.SaveChangesAsync();
        }

        public async Task SaveOpeningAsync(List<InventoryPeriodOpeningEntity> opening)
        {
            await _context.InventoryPeriodOpening.AddRangeAsync(opening);
            await _context.SaveChangesAsync();
        }

        public async Task ClosePeriodAsync(int periodId, int closedByUser)
        {
            var period = await _context.InventoryPeriod
                .FirstOrDefaultAsync(p => p.IdPeriod == periodId);

            if (period is null) return;

            period.Status = 2;
            period.ClosedByUser = closedByUser;
            period.ClosedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task RegisterAdjustmentAsync(int storeId, int periodId, List<InventoryPeriodClosingEntity> closing)
        {
            if (!closing.Any()) return;

            var productIds = closing.Select(c => c.IdProduct).ToList();

            var inventoryItems = await _context.StoreInventory
                .Where(i => i.IdStore == storeId && productIds.Contains(i.IdProduct))
                .ToListAsync();

            foreach (var item in inventoryItems)
            {
                var adjustment = closing.FirstOrDefault(a => a.IdProduct == item.IdProduct);
                if (adjustment is null) continue;

                if (item.StockAvailable != adjustment.ClosingStock)
                    item.StockAvailable = adjustment.ClosingStock;
            }

            await _context.SaveChangesAsync();
        }
    }
}
