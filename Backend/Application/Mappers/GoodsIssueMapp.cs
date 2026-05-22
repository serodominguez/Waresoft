using Application.Dtos.Request.GoodsIssue;
using Application.Dtos.Response.GoodsIssue;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.GoodsIssue;
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

        public static GoodsIssueResponseDto GoodsIssueResponseDtoMapping(GoodsIssueReadModel model)
        {
            return new GoodsIssueResponseDto
            {
                IdIssue = model.Id,
                Code = model.Code,
                Type = model.Type.ToSentenceCase(),
                TotalAmount = model.TotalAmount,
                Annotations = model.Annotations.ToSentenceCaseMultiple(),
                IdUser = model.IdUser,
                UserName = model.Names.ToSentenceCase() + " " + model.LastNames.ToSentenceCase(),
                IdStore = model.IdStore,
                StoreName = model.StoreName.ToTitleCase(),
                AuditCreateUser = model.AuditCreateUser,
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusIssue = ((Movements)(model.Status)).ToString()
            };
        }

        public static GoodsIssueWithDetailsResponseDto GoodsIssueWithDetailsResponseDtoMapping(GoodsIssueWithDetailsReadModel model, string? userName = null)
        {
            return new GoodsIssueWithDetailsResponseDto
            {
                IdIssue = model.Id,
                Code = model.Code,
                Type = model.Type.ToSentenceCase(),
                TotalAmount = model.TotalAmount,
                Annotations = model.Annotations.ToSentenceCaseMultiple(),
                IdUser = model.IdUser,
                UserName = model.Names.ToSentenceCase() + " " + model.LastNames.ToSentenceCase(),
                IdStore = model.IdStore,
                StoreName = model.StoreName.ToTitleCase(),
                AuditCreateUser = model.AuditCreateUser,
                AuditCreateName = userName.ToTitleCase(),
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusIssue = ((Movements)(model.Status)).ToString(),
                GoodsIssueDetails = model.GoodsIssueDetails
                        .Select(d => new GoodsIssueDetailsResponseDto
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
                            UnitPrice = d.UnitPrice,
                            TotalPrice = d.TotalPrice
                        })
                        .ToList()
            };
        }
    }
}
