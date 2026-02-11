using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Transfer;
using Application.Dtos.Response.Transfer;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<TransferRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public TransferService(IUnitOfWork unitOfWork, IValidator<TransferRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<TransferResponseDto>>> ListTransferByStore(int authenticatedStoreId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<TransferResponseDto>>();
            try
            {
                var transfers = _unitOfWork.Transfer.GetTransferQueryableByStore(authenticatedStoreId);

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            transfers = transfers.Where(x => x.Code!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            transfers = transfers.Where(x => x.StoreDestination.StoreName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter.HasValue)
                {
                    var stateValue = Convert.ToInt32(filters.StateFilter);
                    transfers = transfers.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    transfers = transfers.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }
                response.TotalRecords = await transfers.CountAsync();

                filters.Sort ??= "IdTransfer";
                var items = await _orderingQuery.Ordering(filters, transfers, !(bool)filters.Download!).ToListAsync();

                TransferStatusLogic(items, authenticatedStoreId);

                var users = await _unitOfWork.User.GetSelectAsync();
                var userDictionary = users.ToDictionary(
                    u => u.Id,
                    u => $"{u.Names} {u.LastNames}".Trim().ToTitleCase()
                );

                response.IsSuccess = true;
                response.Data = items.Select(x => TransferMapp.TransferResponseDtoMapping(x, userDictionary));
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<TransferWithDetailsResponseDto>> TransferById(int authenticatedStoreId, int trasnferId)
        {
            var response = new BaseResponse<TransferWithDetailsResponseDto>();

            try
            {
                var transfer = await _unitOfWork.Transfer.GetTransferByIdAsync(trasnferId);

                if (transfer is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                string? userSend = null;
                string? userReceive = null;

                if (transfer!.Status == 2)
                {
                    userSend = await GetUserNameByIdAsync(transfer?.AuditCreateUser);
                    userReceive = await GetUserNameByIdAsync(transfer?.AuditUpdateUser);
                }
                else
                {
                    userSend = await GetUserNameByIdAsync(transfer?.AuditCreateUser);
                }

                var details = await _unitOfWork.TransferDetails.GetTransferDetailsAsync(transfer!.IdTransfer);
                transfer.TransferDetails = details.ToList();

                if (transfer.IdStoreDestination == authenticatedStoreId && transfer.Status == 1)
                {
                    transfer.Status = 3;
                }

                response.IsSuccess = true;
                response.Data = TransferMapp.TransferWithDetailsResponseDtoMapping(transfer, userSend, userReceive);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> SendTransfer(int authenticatedUserId, TransferRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            var validationResult = await _validator.ValidateAsync(requestDto);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors;
                return response;
            }

            using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                var entity = TransferMapp.TransferMapping(requestDto);
                entity.Code = await _unitOfWork.Transfer.GenerateCodeAsync();
                entity.SendDate = DateTime.Now;
                entity.AuditCreateUser = authenticatedUserId;
                entity.AuditCreateDate = DateTime.Now;
                entity.Status = 1;
                await _unitOfWork.Transfer.SendTransferAsync(entity);

                foreach (var item in entity.TransferDetails)
                {
                    var currentStock = await _unitOfWork.StoreInventory.GetStockByIdAsync(item.IdProduct, requestDto.IdStoreOrigin);
                    if (currentStock is null)
                    {
                        transaction.Rollback();
                        response.IsSuccess = false;
                        response.Message = ReplyMessage.MESSAGE_NOT_FOUND + item.IdProduct;
                        return response;
                    }
                    else
                    {
                        currentStock.StockAvailable -= item.Quantity;
                        currentStock.StockInTransit += item.Quantity;
                        await _unitOfWork.StoreInventory.UpdateStockByProductsAsync(currentStock);
                    }
                }

                transaction.Commit();
                response.IsSuccess = true;
                response.Data = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> ReceiveTransfer(int authenticatedUserId, int transferId)
        {
            var response = new BaseResponse<bool>();

            using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                var transfer = await _unitOfWork.Transfer.GetTransferByIdAsync(transferId);
                if (transfer is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var details = await _unitOfWork.TransferDetails.GetTransferDetailsAsync(transfer!.IdTransfer);

                transfer.ReceiveDate = DateTime.Now;
                transfer.AuditDeleteUser = authenticatedUserId;
                transfer.AuditDeleteDate = DateTime.Now;
                transfer.Status = 2;

                response.Data = await _unitOfWork.Transfer.ReceiveTransferAsync(transfer);

                foreach (var item in details)
                {
                    var destinationStock = await _unitOfWork.StoreInventory.GetStockByIdAsync(item.IdProduct, transfer.IdStoreDestination);
                    if (destinationStock is not null)
                    {
                        destinationStock.StockAvailable += item.Quantity;
                        await _unitOfWork.StoreInventory.UpdateStockByProductsAsync(destinationStock);
                    }
                    else
                    {
                        var newStock = new StoreInventoryEntity
                        {
                            IdProduct = item.IdProduct,
                            IdStore = transfer.IdStoreDestination,
                            StockAvailable = item.Quantity,
                            StockInTransit = 0,
                            Price = 0
                        };
                        await _unitOfWork.StoreInventory.RegisterStockByProductsAsync(newStock);
                    }

                    var originStock = await _unitOfWork.StoreInventory.GetStockByIdAsync(item.IdProduct, transfer.IdStoreOrigin);
                    if (originStock is null)
                    {
                        transaction.Rollback();
                        response.IsSuccess = false;
                        response.Message = ReplyMessage.MESSAGE_NOT_FOUND + item.IdProduct;
                        return response;
                    }

                    originStock.StockInTransit -= item.Quantity;
                    await _unitOfWork.StoreInventory.UpdateStockByProductsAsync(originStock);
                }

                transaction.Commit();
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> CancelTransfer(int authenticatedUserId, int transferId)
        {
            var response = new BaseResponse<bool>();

            using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                var transfer = await _unitOfWork.Transfer.GetTransferByIdAsync(transferId);
                if (transfer is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var details = await _unitOfWork.TransferDetails.GetTransferDetailsAsync(transfer!.IdTransfer);

                transfer.AuditDeleteUser = authenticatedUserId;
                transfer.AuditDeleteDate = DateTime.Now;
                transfer.Status = 0;

                response.Data = await _unitOfWork.Transfer.CancelTransferAsync(transfer);

                foreach (var item in details)
                {
                    var currentStock = await _unitOfWork.StoreInventory.GetStockByIdAsync(item.IdProduct, transfer.IdStoreOrigin);
                    if (currentStock is null)
                    {
                        transaction.Rollback();
                        response.IsSuccess = false;
                        response.Message = ReplyMessage.MESSAGE_NOT_FOUND + item.IdProduct;
                        return response;
                    }
                    else
                    {
                        currentStock.StockAvailable += item.Quantity;
                        currentStock.StockInTransit -= item.Quantity;
                        await _unitOfWork.StoreInventory.UpdateStockByProductsAsync(currentStock);
                    }
                }

                transaction.Commit();
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        private async Task<string?> GetUserNameByIdAsync(int? userId)
        {
            if (!userId.HasValue) return null;

            var user = await _unitOfWork.User.GetByIdAsync(userId.Value);
            return user?.UserName;
        }

        private void TransferStatusLogic(List<TransferEntity> transfers, int authenticatedStoreId)
        {
            foreach (var transfer in transfers)
            {
                if (transfer.IdStoreDestination == authenticatedStoreId && transfer.Status == 1)
                {
                    transfer.Status = 3;
                }
            }
        }
    }
}
