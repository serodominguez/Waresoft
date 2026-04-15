using Application.Dtos.Request.StoreInventory;
using Application.Dtos.Response.StoreInventory;
using Domain.Entities;
using Domain.Models;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class StoreInventoryMapp
    {
        public static StoreInventoryEntity StoreInventoryMapping (StoreInventoryRequestDto dto)
        {
            return new StoreInventoryEntity
            {
                IdProduct = dto.IdProduct,
                Price = dto.Price
            };
        }

        public static StoreInventoryResponseDto StoreInventoryMapping(StoreInventoryModel item)
        {
            return new StoreInventoryResponseDto
            {
                IdStore = item.IdStore,
                IdProduct = item.IdProduct,
                StockAvailable = item.StockAvailable,
                CalculatedStock = item.CalculatedStock,
                StockDifference = item.StockAvailable - item.CalculatedStock,
                StockInTransit = item.StockInTransit,
                Price = item.Price,
                Replenishment = ((Replenishment)(item.Replenishment!)).ToString().ReplaceUnderscoresWithSpace(),
                Code = item.Code,
                Description = item.Description?.ToSentenceCase(),
                Material = item.Material?.ToSentenceCase(),
                Color = item.Color?.ToSentenceCase(),
                UnitMeasure = item.UnitMeasure?.ToSentenceCase(),
                BrandName = item.BrandName?.ToSentenceCase(),
                CategoryName = item.CategoryName?.ToSentenceCase(),
                AuditCreateDate = item.AuditCreateDate.ToString("dd/MM/yyyy HH:mm")
            };
        }

        public static StoreInventoryPivotResponseDto StoreInventoryPivotMapping(List<InventoryPivotModel> items, List<StoreEntity> stores)
        {
            var storeDict = stores
                .Where(s => !string.IsNullOrEmpty(s.StoreName))
                .ToDictionary(s => s.Id, s => s.StoreName!.ToSentenceCase()!);

            var rows = items.Select(item => new StoreInventoryPivotRowResponseDto
            {
                Image = item.Image,
                Code = item.Code,
                Description = item.Description?.ToSentenceCase(),
                Material = item.Material?.ToSentenceCase(),
                Color = item.Color?.ToSentenceCase(),
                BrandName = item.BrandName?.ToSentenceCase(),
                CategoryName = item.CategoryName?.ToSentenceCase(),
                AuditCreateDate = item.AuditCreateDate.ToString("dd/MM/yyyy HH:mm"),
                StockByStore = ParseStoreStocks(item.StoreStocks, storeDict)
            }).ToList();

            return new StoreInventoryPivotResponseDto
            {
                Stores = storeDict.Values.ToList(),
                Rows = rows
            };
        }

        public static StoreInventoryKardexResponseDto StoreInventoryKardexMapping(StoreInventoryEntity product, List<KardexMovementModel> movements, int calculateStock, int stockDifference)
        {
            return new StoreInventoryKardexResponseDto
            {
                IdProduct = product.IdProduct,
                Code = product.Product?.Code,
                Description = product.Product?.Description?.ToSentenceCase(),
                Material = product.Product?.Material?.ToSentenceCase(),
                Color = product.Product?.Color?.ToSentenceCase(),
                UnitMeasure = product.Product?.UnitMeasure?.ToSentenceCase(),
                CurrentStock = product.StockAvailable,
                CalculatedStock = calculateStock,
                StockDifference = stockDifference,
                Movements = movements.Select(m => new StoreInventoryKardexMovementDto
                {
                    IdProduct = m.IdProduct,
                    Quantity = m.Quantity,
                    IdMovement = m.IdMovement,
                    Code = m.Code,
                    Date = m.Date.ToString("dd/MM/yyyy HH:mm"),
                    MovementType = m.MovementType,
                    Type = m.Type.ToSentenceCase(),
                    State = m.MovementType switch
                    {
                        "Traspaso" => m.State.HasValue
                            ? ((Transfers)m.State.Value).ToString()
                            : string.Empty,

                        _ => m.State.HasValue
                            ? ((Movements)m.State.Value).ToString()
                            : string.Empty
                    },
                    AccumulatedStock = m.AccumulatedStock
                }).ToList()
            };
        }

        private static Dictionary<string, int> ParseStoreStocks(string? storeStocks, Dictionary<int, string> storeDict)
        {
            var result = new Dictionary<string, int>();
            if (string.IsNullOrEmpty(storeStocks)) return result;

            var entries = storeStocks.Split(',');
            foreach (var entry in entries)
            {
                var parts = entry.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out int id) && int.TryParse(parts[1], out int stock))
                {
                    if (storeDict.TryGetValue(id, out var name))
                    {
                        result[name] = stock;
                    }
                }
            }
            return result;
        }
    }
}
