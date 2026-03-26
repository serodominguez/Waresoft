using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Commons.Helpers
{
    public static class StockValidationHelper
    {
        public static async Task<(bool IsValid, string ErrorMessage)> ValidateStockAvailabilityAsync(IUnitOfWork unitOfWork, List<int> productIds, Dictionary<int, int> quantitiesByProduct, int storeId)
        {
            var stocks = await unitOfWork.StoreInventory
                .GetStocksByStoreAsQueryable(storeId)
                .Where(s => productIds.Contains(s.IdProduct))
                .AsNoTracking()
                .ToListAsync();

            foreach (var productId in productIds)
            {
                var stock = stocks.FirstOrDefault(s => s.IdProduct == productId);

                if (stock is null)
                    return (false, ReplyMessage.MESSAGE_NOT_FOUND + " para el producto con Id:" + productId);

                if (stock.StockAvailable < quantitiesByProduct[productId])
                    return (false, ReplyMessage.MESSAGE_STOCK_NOT_AVAILABLE + " para el producto con Id:" + productId);
            }

            return (true, string.Empty);
        }
    }
}
