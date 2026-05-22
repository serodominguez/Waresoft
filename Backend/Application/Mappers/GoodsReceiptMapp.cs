using Application.Dtos.Request.GoodsReceipt;
using Application.Dtos.Response.GoodsReceipt;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.GoodsReceipt;
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
                DocumentDate = !string.IsNullOrWhiteSpace(dto.DocumentDate) ? DateTime.Parse(dto.DocumentDate) : DateTime.MinValue,
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

        public static GoodsReceiptResponseDto GoodsReceiptResponseDtoMapping(GoodsReceiptReadModel model)
        {
            return new GoodsReceiptResponseDto
            {
                IdReceipt = model.Id,
                Code = model.Code,
                Type = model.Type.ToSentenceCase(),
                DocumentDate = model.DocumentDate.HasValue ? model.DocumentDate.Value.ToString("dd/MM/yyyy") : null,
                DocumentType = model.DocumentType.ToSentenceCase(),
                DocumentNumber = model.DocumentNumber,
                TotalAmount = model.TotalAmount,
                Annotations = model.Annotations.ToSentenceCaseMultiple(),
                IdSupplier = model.IdSupplier,
                CompanyName = model.CompanyName.ToTitleCase(),
                IdStore = model.IdStore,
                StoreName = model.StoreName.ToTitleCase(),
                AuditCreateUser = model.AuditCreateUser,
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusReceipt = ((Movements)(model.Status)).ToString()
            };
        }

        public static GoodsReceiptWithDetailsResponseDto GoodsReceiptWithDetailsResponseDtoMapping(GoodsReceiptWithDetailsReadModel model, string? userName = null)
        {
            return new GoodsReceiptWithDetailsResponseDto
            {
                IdReceipt = model.Id,
                Code = model.Code,
                Type = model.Type.ToSentenceCase(),
                DocumentDate = model.DocumentDate.HasValue ? model.DocumentDate.Value.ToString("dd/MM/yyyy") : null,
                DocumentType = model.DocumentType.ToSentenceCase(),
                DocumentNumber = model.DocumentNumber,
                TotalAmount = model.TotalAmount,
                Annotations = model.Annotations.ToSentenceCaseMultiple(),
                IdSupplier = model.IdSupplier,
                CompanyName = model.CompanyName.ToTitleCase(),
                IdStore = model.IdStore,
                StoreName = model.StoreName.ToTitleCase(),
                AuditCreateUser = model.AuditCreateUser,
                AuditCreateName = userName.ToTitleCase(),
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusReceipt = ((Movements)(model.Status)).ToString(),
                GoodsReceiptDetails = model.GoodsReceiptDetails
                        .Select(d => new GoodsReceiptDetailsResponseDto
                        {
                            Item = d.Item,
                            IdProduct = d.IdProduct,
                            Code = d.Code,
                            Description = d.Description.ToSentenceCase(),
                            Material = d.Material.ToSentenceCase(),
                            Color = d.Color.ToSentenceCase(),
                            CategoryName = d.CategoryName.ToSentenceCase(),
                            BrandName = d.BrandName.ToTitleCase(),
                            Quantity = d.Quantity,
                            UnitCost = d.UnitCost,
                            TotalCost = d.TotalCost
                        })
                        .ToList()
            };
        }
    }
}
