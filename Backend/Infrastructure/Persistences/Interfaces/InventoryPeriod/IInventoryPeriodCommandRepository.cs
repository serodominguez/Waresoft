using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces.InventoryPeriod
{
    public interface IInventoryPeriodCommandRepository
    {
        Task<InventoryPeriodEntity> OpenPeriodAsync(InventoryPeriodEntity period);
        Task SaveClosingAsync(List<InventoryPeriodClosingEntity> closing);
        Task SaveOpeningAsync(List<InventoryPeriodOpeningEntity> opening);
        Task RegisterAdjustmentAsync(int storeId, int periodId, List<InventoryPeriodClosingEntity> closing);
        Task ClosePeriodAsync(int periodId, int closedByUser);
    }
}
