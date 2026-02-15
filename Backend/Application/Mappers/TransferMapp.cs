using Application.Dtos.Request.Transfer;
using Application.Dtos.Response.Transfer;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class TransferMapp
    {
        public static TransferEntity TransferMapping(TransferRequestDto dto)
        {
            return new TransferEntity
            {
                TotalAmount = dto.TotalAmount,
                Annotations = dto.Annotations.NormalizeString(),
                IdStoreOrigin = dto.IdStoreOrigin,
                IdStoreDestination = dto.IdStoreDestination,
                TransferDetails = (dto.TransferDetails ?? Enumerable.Empty<TransferDetailsRequestDto>())
                    .Select(details => new TransferDetailsEntity
                    {
                        Item = details.Item,
                        IdProduct = details.IdProduct,
                        Quantity = details.Quantity,
                        UnitPrice = details.UnitPrice,
                        TotalPrice = details.TotalPrice
                    }).ToList()
            };
        }

        public static TransferResponseDto TransferResponseDtoMapping(TransferEntity entity, Dictionary<int, string> userNames)
        {
            return new TransferResponseDto
            {
                IdTransfer = entity.IdTransfer,
                Code = entity.Code,
                SendDate = entity.SendDate.ToString("dd/MM/yyyy HH:mm"),
                ReceiveDate = entity.ReceiveDate.HasValue ? entity.ReceiveDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.ToSentenceCaseMultiple(),
                IdStoreOrigin = entity.IdStoreOrigin,
                StoreOrigin = entity.StoreOrigin.StoreName.ToTitleCase(),
                IdStoreDestination = entity.IdStoreDestination,
                StoreDestination = entity.StoreDestination.StoreName.ToTitleCase(),
                SendUser = entity.AuditCreateUser.HasValue && userNames.TryGetValue(entity.AuditCreateUser.Value, out var nameSend) ? nameSend : string.Empty,
                ReceiveUser = entity.AuditUpdateUser.HasValue && userNames.TryGetValue(entity.AuditUpdateUser.Value, out var nameReceive) ? nameReceive : string.Empty,
                StatusTransfer = ((Transfers)entity.Status).ToString()
            };  
        }

        public static TransferWithDetailsResponseDto TransferWithDetailsResponseDtoMapping(TransferEntity entity, string? userSend = null, string? userReceive = null)
        {
            return new TransferWithDetailsResponseDto
            {
                IdTransfer = entity.IdTransfer,
                Code = entity.Code,
                SendDate = entity.SendDate.ToString("dd/MM/yyyy HH:mm"),
                ReceiveDate = entity.ReceiveDate.HasValue ? entity.ReceiveDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                TotalAmount = entity.TotalAmount,
                Annotations = entity.Annotations.ToSentenceCaseMultiple(),
                IdStoreOrigin = entity.IdStoreOrigin,
                StoreOrigin = entity.StoreOrigin.StoreName.ToTitleCase(),
                IdStoreDestination = entity.IdStoreDestination,
                StoreDestination = entity.StoreDestination.StoreName.ToTitleCase(),
                AuditCreateUser = entity.AuditCreateUser,
                SendUser = userSend.ToTitleCase(),
                AuditUpdateUser = entity.AuditUpdateUser,
                ReceiveUser = userReceive.ToTitleCase(),
                StatusTransfer = ((Transfers)entity.Status).ToString(),
                TransferDetails = entity.TransferDetails
                        .Select(d => new TransferDetailsResponseDto
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
