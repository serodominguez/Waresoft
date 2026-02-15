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
                var inventory = _unitOfWork.StoreInventory.GetInventoryQueryable(authenticatedStoreId);

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            inventory = inventory.Where(x => x.Product.Code!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            inventory = inventory.Where(x => x.Product.Description!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            inventory = inventory.Where(x => x.Product.Material!.Contains(filters.TextFilter));
                            break;
                        case 4:
                            inventory = inventory.Where(x => x.Product.Color!.Contains(filters.TextFilter));
                            break;
                        case 5:
                            inventory = inventory.Where(x => x.Price.ToString().Contains(filters.TextFilter));
                            break;
                        case 6:
                            inventory = inventory.Where(x => x.Product.Brand!.BrandName!.Contains(filters.TextFilter));
                            break;
                        case 7:
                            inventory = inventory.Where(x => x.Product.Category!.CategoryName!.Contains(filters.TextFilter));
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
                    inventory = inventory.Where(x => x.Product.AuditCreateDate >= startDate && x.Product.AuditCreateDate < endDate);
                }
                response.TotalRecords = await inventory.CountAsync();

                filters.Sort ??= "IdProduct";
                var items = await _orderingQuery.Ordering(filters, inventory, true).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(StoreInventoryMapp.StoreInventoryMapping);
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

                var isValid = await _unitOfWork.Product.GetByIdAsync(requestDto.IdProduct);
                if (isValid is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;

                }
                var inventory = StoreInventoryMapp.StoreInventoryMapping(requestDto);
                inventory.IdStore = authenticatedStoreId;
                inventory.AuditUpdateUser = authenticatedUserId;
                inventory.AuditUpdateDate = DateTime.Now;

                response.Data = await _unitOfWork.StoreInventory.UpdatePriceByProductsAsync(inventory);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
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

