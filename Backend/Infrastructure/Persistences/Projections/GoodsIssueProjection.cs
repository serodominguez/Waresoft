using Domain.Entities;
using Infrastructure.Persistences.ReadModels.GoodsIssue;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class GoodsIssueProjection
    {
        public static Expression<Func<GoodsIssueEntity, GoodsIssueReadModel>> ToSummary =>
            i => new GoodsIssueReadModel
            {
                Id = i.Id,
                Code = i.Code,
                Type = i.Type,
                TotalAmount = i.TotalAmount,
                Annotations = i.Annotations,
                IdUser = i.IdUser,
                Names = i.User!.Names,
                LastNames = i.User!.LastNames,
                IdStore = i.IdStore,
                StoreName = i.Store.StoreName,
                AuditCreateUser = i.AuditCreateUser,
                AuditCreateDate = i.AuditCreateDate,
                Status = i.Status
            };

        public static Expression<Func<GoodsIssueEntity, GoodsIssueWithDetailsReadModel>> ToWithDetails =>
            i => new GoodsIssueWithDetailsReadModel
            {
                Id = i.Id,
                Code = i.Code,
                Type = i.Type,
                TotalAmount = i.TotalAmount,
                Annotations = i.Annotations,
                IdUser = i.IdUser,
                Names = i.User!.Names,
                LastNames = i.User!.LastNames,
                IdStore = i.IdStore,
                StoreName = i.Store!.StoreName,
                AuditCreateUser = i.AuditCreateUser,
                AuditCreateDate = i.AuditCreateDate,
                Status = i.Status,
                GoodsIssueDetails = i.GoodsIssueDetails.Select(d => new GoodsIssueDetailsReadModel
                {
                    IdIssue = d.IdIssue,
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

        public static Expression<Func<GoodsIssueDetailsEntity, GoodsIssueDetailsReadModel>> ToDetails =>
            d => new GoodsIssueDetailsReadModel
            {
                IdIssue = d.IdIssue,
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
