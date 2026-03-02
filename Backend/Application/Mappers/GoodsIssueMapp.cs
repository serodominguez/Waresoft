using Application.Dtos.Request.GoodsIssue;
using Application.Dtos.Response.GoodsIssue;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class GoodsIssueMapp
    {
        public static GoodsIssueEntity GoodsIssueMapping(GoodsIssueRequestDto dto)
        {
            return new GoodsIssueEntity
            {
                Type = dto.Type.NormalizeString(),
                TotalAmount = dto.TotalAmount,
                Annotations = dto.Annotations.NormalizeString(),
                IdUser = dto.IdUser,
                IdStore = dto.IdStore,
                GoodsIssueDetails = (dto.GoodsIssueDetails ?? Enumerable.Empty<GoodsIssueDetailsRequestDto>())
                    .Select(details => new GoodsIssueDetailsEntity
                    {
                        Item = details.Item,
                        IdProduct = details.IdProduct,
                        Quantity = details.Quantity,
                        UnitPrice = details.UnitPrice,
                        TotalPrice = details.TotalPrice
                    }).ToList()
            };
        }

        public static GoodsIssueResponseDto GoodsIssueResponseDtoMapping(GoodsIssueEntity entity)
        {
            return new GoodsIssueResponseDto
            {
                IdIssue = entity.IdIssue,
                Code = entity.Code,
                Type = entity.Type.ToSentenceCase(),
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.ToSentenceCaseMultiple(),
                IdUser = entity.IdUser,
                UserName = entity.User.Names.ToSentenceCase() + " " + entity.User.LastNames.ToSentenceCase(),
                IdStore = entity.IdStore,
                StoreName = entity.Store.StoreName.ToTitleCase(),
                AuditCreateUser = entity.AuditCreateUser,
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusIssue = ((Movements)(entity.Status)).ToString()
            };
        }

        public static GoodsIssueWithDetailsResponseDto GoodsIssueWithDetailsResponseDtoMapping(GoodsIssueEntity entity, string? userName = null)
        {
            return new GoodsIssueWithDetailsResponseDto
            {
                IdIssue = entity.IdIssue,
                Code = entity.Code,
                Type = entity.Type.ToSentenceCase(),
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.ToSentenceCaseMultiple(),
                IdUser = entity.IdUser,
                UserName = entity.User.Names.ToSentenceCase() + " " + entity.User.LastNames.ToSentenceCase(),
                IdStore = entity.IdStore,
                StoreName = entity.Store.StoreName.ToTitleCase(),
                AuditCreateUser = entity.AuditCreateUser,
                AuditCreateName = userName.ToTitleCase(),
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusIssue = ((Movements)(entity.Status)).ToString(),
                GoodsIssueDetails = entity.GoodsIssueDetails
                        .Select(d => new GoodsIssueDetailsResponseDto
                        {
                            Item = d.Item,
                            IdProduct = d.IdProduct,
                            Code = d.Product.Code,
                            Description = d.Product.Description.ToSentenceCase(),
                            Material = d.Product.Material.ToSentenceCase(),
                            Color = d.Product.Color.ToSentenceCase(),
                            CategoryName = d.Product.Category.CategoryName.ToSentenceCase(),
                            BrandName = d.Product.Brand.BrandName.ToTitleCase(),
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            TotalPrice = d.TotalPrice
                        })
                        .ToList()
            };
        }
    }
}
