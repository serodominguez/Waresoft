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
                Type = entity.Type.ToTitleCase(),
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.NormalizeString(),
                IdUser = entity.IdUser,
                UserName = entity.User.Names.ToTitleCase() + " " + entity.User.LastNames.ToTitleCase(),
                IdStore = entity.IdStore,
                StoreName = entity.Store.StoreName.ToTitleCase(),
                AuditCreateUser = entity.AuditCreateUser,
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusIssue = ((States)(entity.Status ? 1 : 0)).ToString()
            };
        }

        public static GoodsIssueWithDetailsResponseDto GoodsIssueWithDetailsResponseDtoMapping(GoodsIssueEntity entity, string? userName = null)
        {
            return new GoodsIssueWithDetailsResponseDto
            {
                IdIssue = entity.IdIssue,
                Code = entity.Code,
                Type = entity.Type.ToTitleCase(),
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.NormalizeString(),
                IdUser = entity.IdUser,
                UserName = entity.User.Names.ToTitleCase() + " " + entity.User.LastNames.ToTitleCase(),
                IdStore = entity.IdStore,
                StoreName = entity.Store.StoreName.ToTitleCase(),
                AuditCreateUser = entity.AuditCreateUser,
                AuditCreateName = userName,
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusIssue = ((States)(entity.Status ? 1 : 0)).ToString(),
                GoodsIssueDetails = entity.GoodsIssueDetails
                        .Select(d => new GoodsIssueDetailsResponseDto
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
                            UnitPrice = d.UnitPrice,
                            TotalPrice = d.TotalPrice
                        })
                        .ToList()
            };
        }
    }
}
