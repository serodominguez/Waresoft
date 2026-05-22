using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Transfer;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class TransferProjection
    {
        public static Expression<Func<TransferEntity, TransferReadModel>> ToSummary =>
                   t => new TransferReadModel
                   {
                       Id = t.Id,
                       Code = t.Code,
                       SendDate = t.SendDate,
                       ReceiveDate = t.ReceiveDate,
                       TotalAmount = t.TotalAmount,
                       Annotations = t.Annotations,
                       IdStoreOrigin = t.IdStoreOrigin,
                       StoreOrigin = t.StoreOrigin.StoreName,
                       TypeOrigin = t.StoreOrigin.Type,
                       IdStoreDestination = t.IdStoreDestination,
                       StoreDestination = t.StoreDestination.StoreName,
                       TypeDestination = t.StoreDestination.Type,
                       AuditCreateUser = t.AuditCreateUser,
                       AuditCreateDate = t.AuditCreateDate,
                       AuditUpdateUser = t.AuditUpdateUser,
                       AuditUpdateDate = t.AuditUpdateDate,
                       Status = t.Status
                   };

        public static Expression<Func<TransferEntity, TransferWithDetailsReadModel>> ToWithDetails =>
            t => new TransferWithDetailsReadModel
            {
                Id = t.Id,
                Code = t.Code,
                SendDate = t.SendDate,
                ReceiveDate = t.ReceiveDate,
                TotalAmount = t.TotalAmount,
                Annotations = t.Annotations,
                IdStoreOrigin = t.IdStoreOrigin,
                StoreOrigin = t.StoreOrigin.StoreName,
                TypeOrigin = t.StoreOrigin.Type,
                IdStoreDestination = t.IdStoreDestination,
                StoreDestination = t.StoreDestination.StoreName,
                TypeDestination = t.StoreDestination.Type,
                AuditCreateUser = t.AuditCreateUser,
                AuditUpdateUser = t.AuditUpdateUser,
                Status = t.Status,
                TransferDetails = t.TransferDetails.Select(d => new TransferDetailsReadModel
                {
                    IdTransfer = d.IdTransfer,
                    Item = d.Item,
                    IdProduct = d.IdProduct,
                    Code = d.Product!.Code,
                    Description = d.Product!.Description,
                    Material = d.Product!.Material,
                    Color = d.Product!.Color,
                    CategoryName = d.Product!.Category!.CategoryName,
                    BrandName = d.Product!.Brand!.BrandName,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    TotalPrice = d.TotalPrice
                }).ToList()
            };

        public static Expression<Func<TransferDetailsEntity, TransferDetailsReadModel>> ToDetails =>
            d => new TransferDetailsReadModel
            {
                IdTransfer = d.IdTransfer,
                Item = d.Item,
                IdProduct = d.IdProduct,
                Code = d.Product!.Code,
                Description = d.Product!.Description,
                Material = d.Product!.Material,
                Color = d.Product!.Color,
                CategoryName = d.Product!.Category!.CategoryName,
                BrandName = d.Product!.Brand!.BrandName,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                TotalPrice = d.TotalPrice
            };
    }
}
