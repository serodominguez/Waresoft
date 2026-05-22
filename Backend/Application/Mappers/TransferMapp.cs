using Application.Dtos.Request.Transfer;
using Application.Dtos.Response.Transfer;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Transfer;
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

        public static TransferResponseDto TransferResponseDtoMapping(TransferReadModel model, Dictionary<int, string> userNames, int displayStatus)
        {
            return new TransferResponseDto
            {
                IdTransfer = model.Id,
                Code = model.Code,
                SendDate = model.SendDate.HasValue ? model.SendDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                ReceiveDate = model.ReceiveDate.HasValue ? model.ReceiveDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                TotalAmount = model.TotalAmount,
                Annotations = model.Annotations.ToSentenceCaseMultiple(),
                IdStoreOrigin = model.IdStoreOrigin,
                StoreOrigin = model.TypeOrigin.ToTitleCase() + ' '+ model.StoreOrigin.ToTitleCase(),
                IdStoreDestination = model.IdStoreDestination,
                StoreDestination = model.TypeDestination.ToTitleCase() + ' '+ model.StoreDestination.ToTitleCase(),
                SendUser = model.AuditCreateUser.HasValue && userNames.TryGetValue(model.AuditCreateUser.Value, out var nameSend) ? nameSend : string.Empty,
                ReceiveUser = model.AuditUpdateUser.HasValue && userNames.TryGetValue(model.AuditUpdateUser.Value, out var nameReceive) ? nameReceive : string.Empty,
                StatusTransfer = ((Transfers)displayStatus).ToString()
            };  
        }

        public static TransferWithDetailsResponseDto TransferWithDetailsResponseDtoMapping(TransferWithDetailsReadModel model, List<TransferDetailsReadModel> details, int displayStatus, string? userSend = null, string? userReceive = null)
        {
            return new TransferWithDetailsResponseDto
            {
                IdTransfer = model.Id,
                Code = model.Code,
                SendDate = model.SendDate.HasValue ? model.SendDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                ReceiveDate = model.ReceiveDate.HasValue ? model.ReceiveDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                TotalAmount = model.TotalAmount,
                Annotations = model.Annotations.ToSentenceCaseMultiple(),
                IdStoreOrigin = model.IdStoreOrigin,
                StoreOrigin = model.TypeOrigin.ToTitleCase() + ' ' + model.StoreOrigin.ToTitleCase(),
                IdStoreDestination = model.IdStoreDestination,
                StoreDestination = model.TypeDestination.ToTitleCase() + ' ' + model.StoreDestination.ToTitleCase(),
                AuditCreateUser = model.AuditCreateUser,
                SendUser = userSend.ToTitleCase(),
                AuditUpdateUser = model.AuditUpdateUser,
                ReceiveUser = userReceive.ToTitleCase(),
                StatusTransfer = ((Transfers)displayStatus).ToString(),
                TransferDetails = model.TransferDetails
                        .Select(d => new TransferDetailsResponseDto
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
