using Application.Dtos.Request.GoodsReceipt;
using Application.Dtos.Response.GoodsReceipt;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class GoodsReceiptMapp
    {
        public static GoodsReceiptEntity GoodsReceiptMapping(GoodsReceiptRequestDto dto)
        {
            return new GoodsReceiptEntity
            {
                Type = dto.Type.NormalizeString(),
                DocumentDate = dto.DocumentDate,
                DocumentType = dto.DocumentType.NormalizeString(),
                DocumentNumber = dto.DocumentNumber.NormalizeString(),
                TotalAmount = dto.TotalAmount,
                Annotations = dto.Annotations.NormalizeString(),
                IdSupplier = dto.IdSupplier,
                IdStore = dto.IdStore,
                GoodsReceiptDetails = dto.GoodsReceiptDetails
                    .Select(details => new GoodsReceiptDetailsEntity
                    {
                        Item = details.Item,
                        IdProduct = details.IdProduct,
                        Quantity = details.Quantity,
                        UnitCost = details.UnitCost,
                        TotalCost = details.TotalCost
                    }).ToList()
            };
        }

        public static GoodsReceiptResponseDto GoodsReceiptResponseDtoMapping(GoodsReceiptEntity entity)
        {
            return new GoodsReceiptResponseDto
            {
                IdReceipt = entity.IdReceipt,
                Code = entity.Code,
                Type = entity.Type.ToTitleCase(),
                DocumentDate = entity.DocumentDate.ToString("dd/MM/yyyy"),
                DocumentType = entity.DocumentType.ToTitleCase(),
                DocumentNumber = entity.DocumentNumber,
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.NormalizeString(),
                IdSupplier = entity.IdSupplier,
                CompanyName = entity.Supplier.CompanyName.ToTitleCase(),
                IdStore = entity.IdStore,
                StoreName = entity.Store.StoreName.ToTitleCase(),
                AuditCreateUser = entity.AuditCreateUser,
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusReceipt = ((States)(entity.Status ? 1 : 0)).ToString()
            };
        }

        public static GoodsReceiptWithDetailsResponseDto GoodsReceiptWithDetailsResponseDtoMapping(GoodsReceiptEntity entity, string? userName = null)
        {
            return new GoodsReceiptWithDetailsResponseDto
            {
                IdReceipt = entity.IdReceipt,
                Code = entity.Code,
                Type = entity.Type.ToTitleCase(),
                DocumentDate = entity.DocumentDate.ToString("dd/MM/yyyy"),
                DocumentType = entity.DocumentType.ToTitleCase(),
                DocumentNumber = entity.DocumentNumber,
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.NormalizeString(),
                IdSupplier = entity.IdSupplier,
                CompanyName = entity.Supplier.CompanyName.ToTitleCase(),
                IdStore = entity.IdStore,
                StoreName = entity.Store.StoreName.ToTitleCase(),
                AuditCreateUser = entity.AuditCreateUser,
                AuditCreateName = userName.ToTitleCase(),
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusReceipt = ((States)(entity.Status ? 1 : 0)).ToString(),
                GoodsReceiptDetails = entity.GoodsReceiptDetails
                        .Select(d => new GoodsReceiptDetailsResponseDto
                        {
                            Item = d.Item,
                            IdProduct = d.IdProduct,
                            Code = d.Product.Code,
                            Description = d.Product.Description,
                            Material = d.Product.Material,
                            Color = d.Product.Color,
                            CategoryName = d.Product.Category.CategoryName,
                            BrandName = d.Product.Brand.BrandName,
                            Quantity = d.Quantity,
                            UnitCost = d.UnitCost,
                            TotalCost = d.TotalCost
                        })
                        .ToList()
            };
        }
    }
}
