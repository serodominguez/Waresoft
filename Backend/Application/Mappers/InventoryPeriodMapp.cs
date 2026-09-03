using Application.Dtos.Request.InventoryPeriod;
using Application.Dtos.Response.InventoryPeriod;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.InventoryPeriod;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class InventoryPeriodMapp
    {
        public static InventoryPeriodEntity InventoryPeriodOpenMapping(InventoryPeriodOpenRequestDto dto, int authenticatedUserId, int authenticatedStoreId)
        {
            return new InventoryPeriodEntity
            {
                IdStore = authenticatedStoreId,
                PeriodName = dto.PeriodName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = 1,
                OpenedByUser = authenticatedUserId,
                OpenedDate = DateTime.Now
            };
        }

        public static InventoryPeriodResponseDto InventoryPeriodResponseMapping(InventoryPeriodReadModel model)
        {
            return new InventoryPeriodResponseDto
            {
                IdPeriod = model.IdPeriod,
                IdStore = model.IdStore,
                StoreName = model.StoreName?.ToSentenceCase(),
                PeriodName = model.PeriodName,
                StartDate = model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : null,
                EndDate = model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : null,
                StatusPeriod = ((Periods)(model.Status)).ToString(),
                OpenedByUser = model.OpenedByUser,
                OpenedDate = model.OpenedDate.ToString("dd/MM/yyyy HH:mm"),
                ClosedByUser = model.ClosedByUser,
                ClosedDate = model.ClosedDate.HasValue ? model.ClosedDate.Value.ToString("dd/MM/yyyy HH:mm") : null
            };
        }

        public static InventoryPeriodDetailResponseDto InventoryPeriodDetailResponseMapping(InventoryPeriodDetailReadModel model)
        {
            return new InventoryPeriodDetailResponseDto
            {
                IdPeriod = model.IdPeriod,
                IdStore = model.IdStore,
                StoreName = model.StoreName?.ToSentenceCase(),
                PeriodName = model.PeriodName,
                StartDate = model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : null,
                EndDate = model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : null,
                StatusPeriod = ((Periods)(model.Status)).ToString(),
                OpenedByUser = model.OpenedByUser,
                OpenedDate = model.OpenedDate.ToString("dd/MM/yyyy HH:mm"),
                ClosedByUser = model.ClosedByUser,
                ClosedDate = model.ClosedDate.HasValue ? model.ClosedDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                TotalProducts = model.TotalProducts,
                TotalSystemStock = model.TotalSystemStock,
                TotalPhysicalStock = model.TotalPhysicalStock,
                TotalDifference = model.TotalDifference
            };
        }

        public static InventoryPeriodOpeningResponseDto InventoryPeriodOpeningResponseMapping(InventoryPeriodOpeningReadModel model)
        {
            return new InventoryPeriodOpeningResponseDto
            {
                IdPeriod = model.IdPeriod,
                IdStore = model.IdStore,
                StoreName = model.StoreName?.ToSentenceCase(),
                PeriodName = model.PeriodName,
                StartDate = model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : null,
                EndDate = model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : null,
                StatusPeriod = ((Periods)(model.Status)).ToString(),
                OpenedByUser = model.OpenedByUser,
                OpenedDate = model.OpenedDate.HasValue ? model.OpenedDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                TotalProducts = model.TotalProducts,
                TotalOpeningStock = model.TotalOpeningStock,
                Items = model.Items.Select(o => new InventoryPeriodOpeningItemResponseDto
                {
                    IdProduct = o.IdProduct,
                    ProductCode = o.ProductCode,
                    ProductDescription = o.ProductDescription?.ToSentenceCase(),
                    UnitMeasure = o.UnitMeasure?.ToSentenceCase(),
                    OpeningStock = o.OpeningStock
                }).ToList()
            };
        }

        public static InventoryPeriodClosingResponseDto InventoryPeriodClosingResponseMapping(InventoryPeriodClosingReadModel model)
        {
            return new InventoryPeriodClosingResponseDto
            {
                IdPeriod = model.IdPeriod,
                IdStore = model.IdStore,
                StoreName = model.StoreName?.ToSentenceCase(),
                PeriodName = model.PeriodName,
                StartDate = model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : null,
                EndDate = model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : null,
                StatusPeriod = ((Periods)(model.Status)).ToString(),
                OpenedByUser = model.OpenedByUser,
                OpenedDate = model.OpenedDate.HasValue ? model.OpenedDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                ClosedByUser = model.ClosedByUser,
                ClosedDate = model.ClosedDate.HasValue ? model.ClosedDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                TotalProducts = model.TotalProducts,
                TotalSystemStock = model.TotalSystemStock,
                TotalPhysicalStock = model.TotalPhysicalStock,
                TotalDifference = model.TotalDifference,
                Items = model.Items.Select(c => new InventoryPeriodClosingItemResponseDto
                {
                    IdProduct = c.IdProduct,
                    ProductCode = c.ProductCode,
                    ProductDescription = c.ProductDescription?.ToSentenceCase(),
                    UnitMeasure = c.UnitMeasure?.ToSentenceCase(),
                    SystemStock = c.SystemStock,
                    PhysicalStock = c.PhysicalStock,
                    Difference = c.Difference,
                    ClosingStock = c.ClosingStock
                }).ToList()
            };
        }
    }
}
