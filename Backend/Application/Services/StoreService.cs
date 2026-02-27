using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Store;
using Application.Dtos.Response.Store;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<StoreRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public StoreService(IUnitOfWork unitOfWork, IValidator<StoreRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<StoreResponseDto>>> ListStores(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<StoreResponseDto>>();

            try
            {
                var stores = _unitOfWork.Store.GetAllActiveQueryable()
                    .AsNoTracking();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            stores = stores.Where(x => x.StoreName!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            stores = stores.Where(x => x.Manager!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            stores = stores.Where(x => x.Address!.Contains(filters.TextFilter));
                            break;
                        case 4:
                            stores = stores.Where(x => x.City!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    stores = stores.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                    stores = stores.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }

                response.TotalRecords = await stores.CountAsync();

                filters.Sort ??= "Id";
                var items = await _orderingQuery.Ordering(filters, stores, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(StoreMapp.StoresResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<StoreSelectResponseDto>>> ListSelectStores()
        {
            var response = new BaseResponse<IEnumerable<StoreSelectResponseDto>>();

            try
            {
                var stores = (await _unitOfWork.Store.GetSelectQueryable()
                    .AsNoTracking()
                    .Where(s => s.Status == true)
                    .ToListAsync());

                if (stores is not null && stores.Any())
                {
                    response.Data = stores.Select(StoreMapp.StoresSelectResponseDtoMapping);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<StoreResponseDto>> StoreById(int storeId)
        {
            var response = new BaseResponse<StoreResponseDto>();

            try
            {
                var store = await _unitOfWork.Store.GetByIdAsQueryable(storeId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (store is not null)
                {
                    response.Data = StoreMapp.StoresResponseDtoMapping(store);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterStore(int authenticatedUserId, StoreRequestDto requestDto)
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

                var store = StoreMapp.StoresMapping(requestDto);
                store.AuditCreateUser = authenticatedUserId;
                store.AuditCreateDate = DateTime.Now;
                store.Status = true;

                await _unitOfWork.Store.AddAsync(store);

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_SAVE;
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

        public async Task<BaseResponse<bool>> EditStore(int authenticatedUserId, int storeId, StoreRequestDto requestDto)
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

                var store = await _unitOfWork.Store.GetByIdAsQueryable(storeId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (store is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }


                store.StoreName = requestDto.StoreName.NormalizeString();
                store.Manager = requestDto.Manager.NormalizeString();
                store.Address = requestDto.Address.NormalizeString();
                store.PhoneNumber = requestDto.PhoneNumber;
                store.City = requestDto.City.NormalizeString();
                store.Email = requestDto.Email;
                store.Type = requestDto.Type.NormalizeString();
                store.AuditUpdateUser = authenticatedUserId;
                store.AuditUpdateDate = DateTime.Now;

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

        public async Task<BaseResponse<bool>> EnableStore(int authenticatedUserId, int storeId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var store = await _unitOfWork.Store.GetByIdAsQueryable(storeId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (store is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                store.AuditUpdateUser = authenticatedUserId;
                store.AuditUpdateDate = DateTime.Now;
                store.Status = true;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
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

        public async Task<BaseResponse<bool>> DisableStore(int authenticatedUserId, int storeId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var store = await _unitOfWork.Store.GetByIdAsQueryable(storeId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (store is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                store.AuditUpdateUser = authenticatedUserId;
                store.AuditUpdateDate = DateTime.Now;
                store.Status = false;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
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

        public async Task<BaseResponse<bool>> RemoveStore(int authenticatedUserId, int storeId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var store = await _unitOfWork.Store.GetByIdAsQueryable(storeId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (store is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                store.AuditDeleteUser = authenticatedUserId;
                store.AuditDeleteDate = DateTime.Now;
                store.Status = false;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
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
