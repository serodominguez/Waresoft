using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.GoodsReceipt;
using Application.Dtos.Response.GoodsReceipt;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class GoodsReceiptService : IGoodsReceiptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GoodsReceiptRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public GoodsReceiptService(IUnitOfWork unitOfWork, IValidator<GoodsReceiptRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<GoodsReceiptResponseDto>>> ListGoodsReceiptByStore(int authenticatedStoreId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<GoodsReceiptResponseDto>>();
            try
            {
                var receipts = _unitOfWork.GoodsReceipt.GetGoodsReceiptQueryableByStore(authenticatedStoreId);

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            receipts = receipts.Where(x => x.Code!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            receipts = receipts.Where(x => x.Store.StoreName!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            receipts = receipts.Where(x => x.Supplier.CompanyName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter.HasValue)
                {
                    var stateValue = Convert.ToInt32(filters.StateFilter);

                    switch (stateValue)
                    {
                        case 0:
                            receipts = receipts.Where(x => x.Status == 0);
                            break;

                        case 1:
                            receipts = receipts.Where(x => x.Status == 1);
                            break;

                        case 2:
                            receipts = receipts.Where(x => x.Status >= 0);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    receipts = receipts.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }
                response.TotalRecords = await receipts.CountAsync();

                filters.Sort ??= "IdReceipt";
                var items = await _orderingQuery.Ordering(filters, receipts, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(GoodsReceiptMapp.GoodsReceiptResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<GoodsReceiptWithDetailsResponseDto>> GoodsReceiptById(int receiptId)
        {
            var response = new BaseResponse<GoodsReceiptWithDetailsResponseDto>();

            try
            {
                var receipt = await _unitOfWork.GoodsReceipt.GetGoodsReceiptByIdAsync(receiptId);

                if (receipt is null )
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                string? userName = null;
                if (receipt!.AuditCreateUser.HasValue)
                {
                    var user = await _unitOfWork.User.GetByIdAsync(receipt.AuditCreateUser.Value);
                    userName = user?.Names + ' ' + user?.LastNames;
                }

                var details = await _unitOfWork.GoodsReceiptDetails.GetGoodsReceiptDetailsAsync(receipt!.IdReceipt);

                receipt.GoodsReceiptDetails = details.ToList();

                response.IsSuccess = true;
                response.Data = GoodsReceiptMapp.GoodsReceiptWithDetailsResponseDtoMapping(receipt, userName);
                response.Message= ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterGoodsReceipt(int authenticatedUserId, GoodsReceiptRequestDto requestDto)
        {
            const string TypeReceipt = "Adquisición";
            const string TypeAdjustment = "Ajuste de kardex";

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
                var entity = GoodsReceiptMapp.GoodsReceiptMapping(requestDto);
                entity.Code = await _unitOfWork.GoodsReceipt.GenerateCodeAsync();
                
                if (requestDto.Type != TypeReceipt)
                {
                    entity.DocumentNumber = entity.Code;
                    entity.DocumentDate = DateTime.Now;
                }

                entity.AuditCreateUser = authenticatedUserId;
                entity.AuditCreateDate = DateTime.Now;
                entity.Status = 1;
                entity.IsActive = true;
                await _unitOfWork.GoodsReceipt.RegisterGoodsReceiptAsync(entity);

                if (requestDto.Type != TypeAdjustment)
                {
                    foreach (var item in entity.GoodsReceiptDetails)
                    {
                        var currentStock = await _unitOfWork.StoreInventory.GetStockByIdAsync(item.IdProduct, requestDto.IdStore);

                        if (currentStock is not null)
                        {
                            currentStock.StockAvailable += item.Quantity;
                            currentStock.AuditUpdateUser = authenticatedUserId;
                            currentStock.AuditUpdateDate = DateTime.Now;
                            await _unitOfWork.StoreInventory.UpdateStockByProductsAsync(currentStock);
                        }
                        else
                        {
                            var newStock = new StoreInventoryEntity
                            {
                                IdProduct = item.IdProduct,
                                IdStore = requestDto.IdStore,
                                StockAvailable = item.Quantity,
                                StockInTransit = 0,
                                Price = 0,
                                AuditCreateUser = authenticatedUserId,
                                AuditCreateDate = DateTime.Now
                            };
                            await _unitOfWork.StoreInventory.RegisterStockByProductsAsync(newStock);
                        }

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

        public async Task<BaseResponse<bool>> CancelGoodsReceipt(int authenticatedUserId, int receiptId)
        {
            const string TypeAdjustment = "ajuste de kardex";

            var response = new BaseResponse<bool>();

            using var transaction = _unitOfWork.BeginTransaction();

            try
            {

                var receipt = await _unitOfWork.GoodsReceipt.GetGoodsReceiptByIdAsync(receiptId);
                if (receipt is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                receipt.AuditDeleteUser = authenticatedUserId;
                receipt.AuditDeleteDate = DateTime.Now;
                receipt.Status = 0;
                receipt.IsActive = false;
                response.Data = await _unitOfWork.GoodsReceipt.CancelGoodsReceiptAsync(receipt);


                if (receipt.Type != TypeAdjustment)
                {
                    var details = await _unitOfWork.GoodsReceiptDetails.GetGoodsReceiptDetailsAsync(receipt!.IdReceipt);

                    foreach (var item in details)
                    {
                        var currentStock = await _unitOfWork.StoreInventory.GetStockByIdAsync(item.IdProduct, receipt.IdStore);

                        if (currentStock is null)
                        {
                            transaction.Rollback();
                            response.IsSuccess = false;
                            response.Message = ReplyMessage.MESSAGE_NOT_FOUND + "para el Id:" + item.IdProduct;
                            return response;
                        }
                        currentStock.StockAvailable -= item.Quantity;
                        currentStock.AuditUpdateUser = authenticatedUserId;
                        currentStock.AuditUpdateDate = DateTime.Now;
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
    }
}
