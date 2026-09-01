using Domain.Entities;
using Infrastructure.Persistences.ReadModels.InventoryPeriod;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class InventoryPeriodProjection
    {
        public static Expression<Func<InventoryPeriodEntity, InventoryPeriodReadModel>> ToSummary =>
            p => new InventoryPeriodReadModel
            {
                IdPeriod = p.IdPeriod,
                IdStore = p.IdStore,
                StoreName = p.Store.StoreName,
                PeriodName = p.PeriodName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                OpenedByUser = p.OpenedByUser,
                OpenedDate = p.OpenedDate,
                ClosedByUser = p.ClosedByUser,
                ClosedDate = p.ClosedDate
            };

        public static Expression<Func<InventoryPeriodEntity, InventoryPeriodDetailReadModel>> ToDetail =>
            p => new InventoryPeriodDetailReadModel
            {
                IdPeriod = p.IdPeriod,
                IdStore = p.IdStore,
                StoreName = p.Store.StoreName,
                PeriodName = p.PeriodName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                OpenedByUser = p.OpenedByUser,
                OpenedDate = p.OpenedDate,
                ClosedByUser = p.ClosedByUser,
                ClosedDate = p.ClosedDate,
                TotalProducts = p.InventoryPeriodClosing.Count,
                TotalSystemStock = p.InventoryPeriodClosing.Sum(c => c.SystemStock),
                TotalPhysicalStock = p.InventoryPeriodClosing.Sum(c => c.PhysicalStock ?? 0),
                TotalDifference = p.InventoryPeriodClosing.Sum(c => c.Difference ?? 0)
            };


        public static Expression<Func<InventoryPeriodEntity, InventoryPeriodOpeningReadModel>> ToOpeningSummary =>
            p => new InventoryPeriodOpeningReadModel
            {
                IdPeriod = p.IdPeriod,
                IdStore = p.IdStore,
                StoreName = p.Store.StoreName,
                PeriodName = p.PeriodName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                OpenedByUser = p.OpenedByUser,
                OpenedDate = p.OpenedDate,
                TotalProducts = p.InventoryPeriodOpening.Count,
                TotalOpeningStock = p.InventoryPeriodOpening.Sum(o => o.OpeningStock),
                Items = p.InventoryPeriodOpening.Select(o => new InventoryPeriodOpeningItemReadModel
                {
                    IdPeriod = o.IdPeriod,
                    IdProduct = o.IdProduct,
                    ProductCode = o.Product.Code,
                    ProductDescription = o.Product.Description,
                    UnitMeasure = o.Product.UnitMeasure,
                    OpeningStock = o.OpeningStock
                }).ToList()
            };

        public static Expression<Func<InventoryPeriodEntity, InventoryPeriodClosingReadModel>> ToClosingSummary =>
            p => new InventoryPeriodClosingReadModel
            {
                IdPeriod = p.IdPeriod,
                IdStore = p.IdStore,
                StoreName = p.Store.StoreName,
                PeriodName = p.PeriodName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                OpenedByUser = p.OpenedByUser,
                OpenedDate = p.OpenedDate,
                ClosedByUser = p.ClosedByUser,
                ClosedDate = p.ClosedDate,
                TotalProducts = p.InventoryPeriodClosing.Count,
                TotalSystemStock = p.InventoryPeriodClosing.Sum(c => c.SystemStock),
                TotalPhysicalStock = p.InventoryPeriodClosing.Sum(c => c.PhysicalStock ?? 0),
                TotalDifference = p.InventoryPeriodClosing.Sum(c => c.Difference ?? 0),
                Items = p.InventoryPeriodClosing.Select(c => new InventoryPeriodClosingItemReadModel
                {
                    IdPeriod = c.IdPeriod,
                    IdProduct = c.IdProduct,
                    ProductCode = c.Product.Code,
                    ProductDescription = c.Product.Description,
                    UnitMeasure = c.Product.UnitMeasure,
                    SystemStock = c.SystemStock,
                    PhysicalStock = c.PhysicalStock,
                    Difference = c.Difference,
                    ClosingStock = c.ClosingStock
                }).ToList()
            };

        public static Expression<Func<InventoryPeriodOpeningEntity, InventoryPeriodOpeningItemReadModel>> ToOpeningItem =>
            o => new InventoryPeriodOpeningItemReadModel
            {
                IdPeriod = o.IdPeriod,
                IdProduct = o.IdProduct,
                ProductCode = o.Product.Code,
                ProductDescription = o.Product.Description,
                UnitMeasure = o.Product.UnitMeasure,
                OpeningStock = o.OpeningStock
            };

        public static Expression<Func<InventoryPeriodClosingEntity, InventoryPeriodClosingItemReadModel>> ToClosingItem =>
            c => new InventoryPeriodClosingItemReadModel
            {
                IdPeriod = c.IdPeriod,
                IdProduct = c.IdProduct,
                ProductCode = c.Product.Code,
                ProductDescription = c.Product.Description,
                UnitMeasure = c.Product.UnitMeasure,
                SystemStock = c.SystemStock,
                PhysicalStock = c.PhysicalStock,
                Difference = c.Difference,
                ClosingStock = c.ClosingStock
            };
    }
}
