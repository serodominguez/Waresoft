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
                Annotations = entity.Annotations.NormalizeString(),
                IdStoreOrigin = entity.IdStoreOrigin,
                StoreOrigin = entity.StoreOrigin.StoreName.ToTitleCase(),
                IdStoreDestination = entity.IdStoreDestination,
                StoreDestination = entity.StoreDestination.StoreName.ToTitleCase(),
                UserName = entity.AuditCreateUser.HasValue && userNames.TryGetValue(entity.AuditCreateUser.Value, out var name) ? name : string.Empty,
                Status = entity.Status,
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
                Annotations = entity.Annotations.NormalizeString(),
                IdStoreOrigin = entity.IdStoreOrigin,
                StoreOrigin = entity.StoreOrigin.StoreName.ToTitleCase(),
                IdStoreDestination = entity.IdStoreDestination,
                StoreDestination = entity.StoreDestination.StoreName.ToTitleCase(),
                AuditCreateUser = entity.AuditCreateUser,
                AuditCreateName = userSend,
                AuditUpdateUser = entity.AuditUpdateUser,
                AuditUpdateName = userReceive,
                StatusTransfer = ((Transfers)entity.Status).ToString(),
                TransferDetails = entity.TransferDetails
                        .Select(d => new TransferDetailsResponseDto
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
