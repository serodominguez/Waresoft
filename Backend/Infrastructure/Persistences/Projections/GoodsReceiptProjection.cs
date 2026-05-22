using Domain.Entities;
using Infrastructure.Persistences.ReadModels.GoodsReceipt;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class GoodsReceiptProjection
    {
        public static Expression<Func<GoodsReceiptEntity, GoodsReceiptReadModel>> ToSummary =>
            r => new GoodsReceiptReadModel
            {
                Id = r.Id,
                Code = r.Code,
                Type = r.Type,
                DocumentDate = r.DocumentDate,
                DocumentType = r.DocumentType,
                DocumentNumber = r.DocumentNumber,
                TotalAmount = r.TotalAmount,
                Annotations = r.Annotations,
                IdSupplier = r.IdSupplier,
                CompanyName = r.Supplier!.CompanyName,
                IdStore = r.IdStore,
                StoreName = r.Store.StoreName,
                AuditCreateUser = r.AuditCreateUser,
                AuditCreateDate = r.AuditCreateDate,
                Status = r.Status
            };

        public static Expression<Func<GoodsReceiptEntity, GoodsReceiptWithDetailsReadModel>> ToWithDetails =>
            r => new GoodsReceiptWithDetailsReadModel
            {
                Id = r.Id,
                Code = r.Code,
                Type = r.Type,
                DocumentDate = r.DocumentDate,
                DocumentType = r.DocumentType,
                DocumentNumber = r.DocumentNumber,
                TotalAmount = r.TotalAmount,
                Annotations = r.Annotations,
                IdSupplier = r.IdSupplier,
                CompanyName = r.Supplier!.CompanyName,
                IdStore = r.IdStore,
                StoreName = r.Store.StoreName,
                AuditCreateUser = r.AuditCreateUser,
                AuditCreateDate = r.AuditCreateDate,
                Status = r.Status,
                GoodsReceiptDetails = r.GoodsReceiptDetails.Select(d => new GoodsReceiptDetailsReadModel
                {
                    IdReceipt = d.IdReceipt,
                    Item = d.Item,
                    IdProduct = d.IdProduct,
                    Code = d.Product!.Code,
                    Description = d.Product!.Description,
                    Material = d.Product!.Material,
                    Color = d.Product!.Color,
                    CategoryName = d.Product!.Category!.CategoryName,
                    BrandName = d.Product!.Brand!.BrandName,
                    Quantity = d.Quantity,
                    UnitCost = d.UnitCost,
                    TotalCost = d.TotalCost
                }).ToList()
            };

        public static Expression<Func<GoodsReceiptDetailsEntity, GoodsReceiptDetailsReadModel>> ToDetails =>
            d => new GoodsReceiptDetailsReadModel
            {
                IdReceipt = d.IdReceipt,
                Item = d.Item,
                IdProduct = d.IdProduct,
                Code = d.Product!.Code,
                Description = d.Product!.Description,
                Material = d.Product!.Material,
                Color = d.Product!.Color,
                CategoryName = d.Product!.Category!.CategoryName,
                BrandName = d.Product!.Brand!.BrandName,
                Quantity = d.Quantity,
                UnitCost = d.UnitCost,
                TotalCost = d.TotalCost
            };
    }
}
