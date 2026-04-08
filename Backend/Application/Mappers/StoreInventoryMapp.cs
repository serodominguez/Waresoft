using Application.Dtos.Request.StoreInventory;
using Application.Dtos.Response.StoreInventory;
using Domain.Entities;
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

        public static StoreInventoryResponseDto StoreInventoryMapping(StoreInventoryEntity entity)
        {
            return new StoreInventoryResponseDto
            {
                IdStore = entity.IdStore,
                IdProduct = entity.IdProduct,
                StockAvailable = entity.StockAvailable,
                StockInTransit = entity.StockInTransit,
                Price = entity.Price,
                Replenishment = ((Replenishment)(entity.Product.Replenishment)).ToString().ReplaceUnderscoresWithSpace(),
                Code = entity.Product?.Code,
                Description = entity.Product?.Description.ToSentenceCase(),
                Material = entity.Product?.Material.ToSentenceCase(),
                Color = entity.Product?.Color.ToSentenceCase(),
                UnitMeasure = entity.Product?.UnitMeasure.ToSentenceCase(),
                BrandName = entity.Product?.Brand?.BrandName.ToSentenceCase(),
                CategoryName = entity.Product?.Category?.CategoryName.ToSentenceCase(),
                AuditCreateDate = entity.Product?.AuditCreateDate?.ToString("dd/MM/yyyy HH:mm"),
            };
        }

        public static StoreInventoryPivotResponseDto StoreInventoryPivotMapping(List<StoreInventoryEntity> inventory, List<StoreEntity> stores)
        {
            // 1. Diccionario ID → Nombre (búsqueda rápida)
            var storeDict = stores
                .Where(s => !string.IsNullOrEmpty(s.StoreName))
                .ToDictionary(s => s.Id, s => s.StoreName!.ToSentenceCase()!);

            var rows = inventory
                .GroupBy(i => i.Product.Id)
                .Select(g =>
                {
                    var product = g.First().Product;

                    // 2. Primero creas Dictionary<int, int> (búsqueda por ID - RÁPIDO)
                    var stockByStoreId = g.ToDictionary(i => i.IdStore, i => i.StockAvailable);

                    // 3. Al final transformas a Dictionary<string, int> para el JSON
                    var stockByStoreName = new Dictionary<string, int>();

                    foreach (var kvp in stockByStoreId)
                    {
                        if (storeDict.TryGetValue(kvp.Key, out var storeName))
                        {
                            // Manejar duplicados agregando ID si es necesario
                            var key = storeName;
                            var suffix = 1;
                            while (stockByStoreName.ContainsKey(key))
                            {
                                key = $"{storeName} ({kvp.Key})";  // Agregar ID si hay duplicado
                                suffix++;
                            }
                            stockByStoreName[key] = kvp.Value;
                        }
                    }

                    return new StoreInventoryPivotRowResponseDto
                    {
                        Image = product.Image,
                        Code = product.Code,
                        Color = product.Color?.ToSentenceCase(),
                        BrandName = product.Brand?.BrandName?.ToSentenceCase(),
                        CategoryName = product.Category?.CategoryName?.ToSentenceCase(),
                        AuditCreateDate = product.AuditCreateDate?.ToString("dd/MM/yyyy HH:mm"),
                        StockByStore = stockByStoreName  // ← Dictionary<string, int>
                    };
                })
                .ToList();

            return new StoreInventoryPivotResponseDto
            {
                Stores = storeDict.Values.ToList(),
                Rows = rows
            };
        }

        public static StoreInventoryKardexResponseDto StoreInventoryKardexMapping(StoreInventoryEntity product, List<StoreInventoryKardexMovementDto> movements, int calculateStock, int stockDifference)
        {
            return new StoreInventoryKardexResponseDto
            {
                IdProduct = product.IdProduct,
                Code = product?.Product.Code ?? string.Empty,
                Description = product?.Product.Description?.ToSentenceCase() ?? string.Empty,
                Material = product?.Product.Material?.ToSentenceCase(),
                Color = product?.Product.Color?.ToSentenceCase(),
                UnitMeasure = product?.Product.UnitMeasure?.ToSentenceCase(),
                CurrentStock = product!.StockAvailable,
                CalculatedStock = calculateStock,
                StockDifference = stockDifference,
                Movements = movements ?? new List<StoreInventoryKardexMovementDto>()
            };
        }

        public static StoreInventoryKardexMovementDto MapReceiptToKardexMovement(GoodsReceiptDetailsEntity receipt)
        {
            return new StoreInventoryKardexMovementDto
            {
                IdProduct = receipt.IdProduct,
                Quantity = receipt.Quantity,
                IdMovement = receipt.IdReceipt,
                Code = receipt.GoodsReceipt.Code,
                Date = receipt.GoodsReceipt!.AuditCreateDate.HasValue ? receipt.GoodsReceipt.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                MovementType = "Entrada",
                Type = receipt.GoodsReceipt.Type?.ToSentenceCase(),
                State = ((Movements)receipt.GoodsReceipt.Status).ToString(),
                AccumulatedStock = 0
            };
        }

        public static StoreInventoryKardexMovementDto MapIssueToKardexMovement(GoodsIssueDetailsEntity issue)
        {
            return new StoreInventoryKardexMovementDto
            {
                IdProduct = issue.IdProduct,
                Quantity = -issue.Quantity,
                IdMovement = issue.IdIssue,
                Code = issue.GoodsIssue.Code,
                Date = issue.GoodsIssue!.AuditCreateDate.HasValue ? issue.GoodsIssue.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                MovementType = "Salida",
                Type = issue.GoodsIssue.Type.ToSentenceCase(),
                State = ((Movements)issue.GoodsIssue.Status).ToString(),
                AccumulatedStock = 0
            };
        }

        public static StoreInventoryKardexMovementDto MapTransferToKardexMovement(TransferDetailsEntity transfer, int authenticatedStoreId)
        {
            bool isOrigin = transfer.Transfer?.IdStoreOrigin == authenticatedStoreId;

            return new StoreInventoryKardexMovementDto
            {
                IdProduct = transfer.IdProduct,
                Quantity = isOrigin ? -transfer.Quantity : transfer.Quantity,
                IdMovement = transfer.IdTransfer,
                Code = transfer.Transfer!.Code,
                Date = isOrigin
                    ? (transfer.Transfer!.SendDate.ToString("dd/MM/yyyy HH:mm"))
                    : (transfer.Transfer!.ReceiveDate.HasValue ? transfer.Transfer.ReceiveDate.Value.ToString("dd/MM/yyyy HH:mm") : null),
                MovementType = "Traspaso",
                Type = isOrigin ? "Salida" : "Entrada",
                State = ((Transfers)transfer.Transfer.Status).ToString().ReplaceUnderscoresWithSpace(),
                AccumulatedStock = 0
            };
        }

    }
}
