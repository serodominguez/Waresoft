using Application.Dtos.Request.StoreInventory;
using Application.Dtos.Response.StoreInventory;
using DocumentFormat.OpenXml.Vml.Office;
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

        public static StoreInventoryPivotResponseDto StoreInventoryPivotMapping(List<StoreInventoryEntity> inventory, List<StoreEntity> entity)
        {
            var stores = entity
                .Where(s => !string.IsNullOrEmpty(s.StoreName))
                .Select(s => s.StoreName.ToSentenceCase()!)
                .Distinct()
                .ToList();

            var rows = inventory
                .GroupBy(i => i.Product.Id)
                .Select(g => new StoreInventoryPivotRowResponseDto
                {
                    Code = g.First().Product.Code,
                    Color = g.First().Product.Color.ToSentenceCase(),
                    BrandName = g.First().Product.Brand.BrandName.ToSentenceCase(),
                    CategoryName = g.First().Product.Category.CategoryName.ToSentenceCase(),
                    AuditCreateDate = g.First().Product.AuditCreateDate?.ToString("dd/MM/yyyy HH:mm"),
                    StockByStore = stores.ToDictionary(
                        store => store,
                        store => g.FirstOrDefault(i => i.Store.StoreName.ToSentenceCase() == store)?.StockAvailable ?? 0
                    )
                }).ToList();

            return new StoreInventoryPivotResponseDto { Stores = stores, Rows = rows };
        }

        public static StoreInventoryKardexResponseDto StoreInventoryKardexMapping(StoreInventoryEntity product, List<StoreInventoryKardexMovementDto> movements, int currentStock)
        {
            return new StoreInventoryKardexResponseDto
            {
                ProductDescription = product?.Product.Description?.ToSentenceCase() ?? string.Empty,
                ProductCode = product?.Product.Code ?? string.Empty,
                CurrentStock = currentStock,
                Movements = movements ?? new List<StoreInventoryKardexMovementDto>()
            };
        }

        public static StoreInventoryKardexMovementDto MapReceiptToKardexMovement(GoodsReceiptDetailsEntity receipt)
        {
            return new StoreInventoryKardexMovementDto
            {
                Quantity = receipt.Quantity,
                Code = receipt.GoodsReceipt.Code,
                Date = receipt.GoodsReceipt!.AuditCreateDate.HasValue ? receipt.GoodsReceipt.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Type = receipt.GoodsReceipt.Type?.ToSentenceCase(),
                State = receipt.GoodsReceipt != null
                    ? ((Movements)(receipt.GoodsReceipt.Status)).ToString()
                    : string.Empty,
                MovementType = "ENTRADA",
                Stock = 0
            };
        }

        public static StoreInventoryKardexMovementDto MapIssueToKardexMovement(GoodsIssueDetailsEntity issue)
        {
            return new StoreInventoryKardexMovementDto
            {
                Quantity = -issue.Quantity,
                Code = issue.GoodsIssue.Code,
                Date = issue.GoodsIssue!.AuditCreateDate.HasValue ? issue.GoodsIssue.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Type = issue.GoodsIssue.Type.ToSentenceCase(),
                State = issue.GoodsIssue != null
                    ? ((Movements)(issue.GoodsIssue.Status)).ToString()
                    : string.Empty,
                MovementType = "SALIDA",
                Stock = 0
            };
        }

        public static StoreInventoryKardexMovementDto MapTransferToKardexMovement(
            TransferDetailsEntity transfer,
            int authenticatedStoreId)
        {
            bool isOrigin = transfer.Transfer?.IdStoreOrigin == authenticatedStoreId;

            return new StoreInventoryKardexMovementDto
            {
                Quantity = isOrigin ? -transfer.Quantity : transfer.Quantity,
                Code = transfer.Transfer!.Code,
                Date = isOrigin
                    ? (transfer.Transfer!.SendDate.ToString("dd/MM/yyyy HH:mm"))
                    : (transfer.Transfer!.ReceiveDate.HasValue ? transfer.Transfer.ReceiveDate.Value.ToString("dd/MM/yyyy HH:mm") : null),
                Type = "TRASPASO",
                State = transfer.Transfer != null
                    ? ((Transfers)(transfer.Transfer.Status)).ToString().ReplaceUnderscoresWithSpace()
                    : string.Empty,
                MovementType = isOrigin ? "SALIDA" : "ENTRADA",
                Stock = 0
            };
        }

    }
}
