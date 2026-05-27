using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.StoreInventory;
using Application.Dtos.Response.StoreInventory;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Utilities.Static;

namespace Application.Services
{
    public class StoreInventoryService : IStoreInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<StoreInventoryRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public StoreInventoryService(IUnitOfWork unitOfWork, IValidator<StoreInventoryRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<StoreInventoryResponseDto>>> ListInventory(int authenticatedStoreId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<StoreInventoryResponseDto>>();
            try
            {
                var inventory = _unitOfWork.StoreInventoryQuery.GetInventoryListQueryable(authenticatedStoreId)
                    .Where(i => i.IsActive == true || i.StockAvailable != 0);

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            inventory = inventory.Where(x => x.Code!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            inventory = inventory.Where(x => x.Description!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            inventory = inventory.Where(x => x.Material!.Contains(filters.TextFilter));
                            break;
                        case 4:
                            inventory = inventory.Where(x => x.Color!.Contains(filters.TextFilter));
                            break;
                        case 5:
                            inventory = inventory.Where(x => x.Price.ToString().Contains(filters.TextFilter));
                            break;
                        case 6:
                            inventory = inventory.Where(x => x.BrandName!.Contains(filters.TextFilter));
                            break;
                        case 7:
                            inventory = inventory.Where(x => x.CategoryName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                //if (filters.StateFilter is not null)
                //{
                //    var stateValue = Convert.ToBoolean(filters.StateFilter);
                //    inventory = inventory.Where(x => x.Product.Status == stateValue);
                //}

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    inventory = inventory.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }
                response.TotalRecords = await inventory.CountAsync();

                filters.Sort ??= "IdProduct";
                var items = await _orderingQuery.Ordering(filters, inventory, true).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(StoreInventoryMapp.StoreInventoryResponseMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<StoreInventoryCalculatedResponseDto>>> ListInventoryCalculated(int authenticatedStoreId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<StoreInventoryCalculatedResponseDto>>();
            try
            {
                DateTime? startDate = string.IsNullOrEmpty(filters.StartDate)
                    ? null : Convert.ToDateTime(filters.StartDate).Date;

                DateTime? endDate = string.IsNullOrEmpty(filters.EndDate)
                    ? null : Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                bool? stateFilter = filters.StateFilter != null
                    ? Convert.ToBoolean(filters.StateFilter) : null;

                bool isDownload = filters.Download ?? false;
                int pageNumber = isDownload ? 1 : filters.NumberPage;
                int pageSize = isDownload ? int.MaxValue : filters.NumberRecordsPage;

                var (items, total) = await _unitOfWork.StoreInventoryQuery.GetInventoryCalculatedAsync(
                    authenticatedStoreId,
                    filters.NumberFilter,
                    filters.TextFilter,
                    stateFilter,
                    startDate,
                    endDate,
                    pageNumber,
                    pageSize);

                response.TotalRecords = total;
                response.Data = items.Select(StoreInventoryMapp.StoreInventoryCalculatedMapping);

                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<StoreInventoryPivotResponseDto>> ListInventoryPivot(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<StoreInventoryPivotResponseDto>();

            DateTime? start = string.IsNullOrEmpty(filters.StartDate) ? null : Convert.ToDateTime(filters.StartDate);
            DateTime? end = string.IsNullOrEmpty(filters.EndDate) ? null : Convert.ToDateTime(filters.EndDate).AddDays(1);
            bool? state = filters.StateFilter != null ? Convert.ToBoolean(filters.StateFilter) : null;

            bool isDownload = filters.Download ?? false;
            int pageNumber = isDownload ? 1 : filters.NumberPage;
            int pageSize = isDownload ? int.MaxValue : filters.NumberRecordsPage;

            var (items, total) = await _unitOfWork.StoreInventoryQuery.GetInventoryPivotAsync(
                filters.NumberFilter,
                filters.TextFilter,
                state, 
                start, 
                end,
                pageNumber,
                pageSize);

            var stores = await _unitOfWork.StoreQuery.GetStoresListQueryable().ToListAsync();

            response.Data = StoreInventoryMapp.StoreInventoryPivotMapping(items, stores);
            response.TotalRecords = total;
            response.IsSuccess = true;
            response.Message = ReplyMessage.MESSAGE_QUERY;

            return response;
        }

        public async Task<BaseResponse<StoreInventoryKardexResponseDto>> ListKardexInventory(int authenticatedStoreId, int productId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<StoreInventoryKardexResponseDto>();
            try
            {
                var inventory = await _unitOfWork.StoreInventoryQuery
                    .GetInventoryListQueryable(authenticatedStoreId)
                    .FirstOrDefaultAsync(i => i.IdProduct == productId);

                if (inventory is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                DateTime? startDate = string.IsNullOrEmpty(filters.StartDate)
                    ? null : Convert.ToDateTime(filters.StartDate).Date;

                DateTime? endDate = string.IsNullOrEmpty(filters.EndDate)
                    ? null : Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                var movements = await _unitOfWork.StoreInventoryQuery
                    .GetKardexByProductAsync(authenticatedStoreId, productId, startDate, endDate);

                response.TotalRecords = movements.Count;

                var paginatedMovements = !(bool)filters.Download!
                    ? movements
                        .Skip((filters.NumberPage - 1) * filters.NumberRecordsPage)
                        .Take(filters.NumberRecordsPage)
                        .ToList()
                    : movements;

                var calculatedStock = movements.LastOrDefault()?.AccumulatedStock ?? 0;
                var stockDifference = inventory.StockAvailable - calculatedStock;

                response.Data = StoreInventoryMapp.StoreInventoryKardexMapping(inventory, paginatedMovements, calculatedStock, stockDifference);

                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> UpdatePriceByProduct(int authenticatedUserId, int authenticatedStoreId, StoreInventoryRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var validationResult = await _validator.ValidateAsync(requestDto);

                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    response.Errors = validationResult.Errors;
                    return response;
                }

                var inventory = await _unitOfWork.StoreInventoryCommand.GetStockByIdAsQueryable(requestDto.IdProduct, authenticatedStoreId)
                    .FirstOrDefaultAsync();

                if (inventory is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;

                }

                inventory.IdStore = authenticatedStoreId;
                inventory.Price = requestDto.Price;
                inventory.AuditUpdateUser = authenticatedUserId;
                inventory.AuditUpdateDate = DateTime.Now;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }
    }
}

