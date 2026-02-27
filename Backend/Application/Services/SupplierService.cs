using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Supplier;
using Application.Dtos.Response.Supplier;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<SupplierRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public SupplierService(IUnitOfWork unitOfWork, IValidator<SupplierRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<SupplierResponseDto>>> ListSuppliers(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<SupplierResponseDto>>();

            try
            {
                var suppliers = _unitOfWork.Supplier.GetAllActiveQueryable()
                    .AsNoTracking();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            suppliers = suppliers.Where(x => x.CompanyName!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            suppliers = suppliers.Where(x => x.Contact!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    suppliers = suppliers.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                    suppliers = suppliers.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }

                response.TotalRecords = await suppliers.CountAsync();

                filters.Sort ??= "Id";
                var items = await _orderingQuery.Ordering(filters, suppliers, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(SupplierMapp.SuppliersResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }
        public async Task<BaseResponse<IEnumerable<SupplierSelectResponseDto>>> ListSelectSuppliers()
        {
            var response = new BaseResponse<IEnumerable<SupplierSelectResponseDto>>();

            try
            {
                var suppliers = (await _unitOfWork.Supplier.GetSelectQueryable()
                    .AsNoTracking()
                    .Where(s => s.Status == true)
                    .ToListAsync());

                if (suppliers is not null && suppliers.Any())
                {
                    response.Data = suppliers.Select(SupplierMapp.SuppliersSelectResponseDtoMapping);
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

        public async Task<BaseResponse<SupplierResponseDto>> SupplierById(int supplierId)
        {
            var response = new BaseResponse<SupplierResponseDto>();

            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsQueryable(supplierId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (supplier is not null)
                {
                    response.Data = SupplierMapp.SuppliersResponseDtoMapping(supplier);
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

        public async Task<BaseResponse<bool>> RegisterSupplier(int authenticatedUserId, SupplierRequestDto requestDto)
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

                var supplier = SupplierMapp.SuppliersMapping(requestDto);
                supplier.AuditCreateUser = authenticatedUserId;
                supplier.AuditCreateDate = DateTime.Now;
                supplier.Status = true;

                await _unitOfWork.Supplier.AddAsync(supplier);

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

        public async Task<BaseResponse<bool>> EditSupplier(int authenticatedUserId, int supplierId, SupplierRequestDto requestDto)
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

                var supplier = await _unitOfWork.Supplier.GetByIdAsQueryable(supplierId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (supplier is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                supplier.CompanyName = requestDto.CompanyName.NormalizeString();
                supplier.Contact = requestDto.Contact.NormalizeString();
                supplier.PhoneNumber = requestDto.PhoneNumber;
                supplier.Email = requestDto.Email; 
                supplier.AuditUpdateUser = authenticatedUserId;
                supplier.AuditUpdateDate = DateTime.Now;

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

        public async Task<BaseResponse<bool>> EnableSupplier(int authenticatedUserId, int supplierId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsQueryable(supplierId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (supplier is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                supplier.AuditUpdateUser = authenticatedUserId;
                supplier.AuditUpdateDate = DateTime.Now;
                supplier.Status = true;

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

        public async Task<BaseResponse<bool>> DisableSupplier(int authenticatedUserId, int supplierId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsQueryable(supplierId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (supplier is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                supplier.AuditUpdateUser = authenticatedUserId;
                supplier.AuditUpdateDate = DateTime.Now;
                supplier.Status = false;

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

        public async Task<BaseResponse<bool>> RemoveSupplier(int authenticatedUserId, int supplierId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var supplier = await _unitOfWork.Supplier.GetByIdAsQueryable(supplierId)
                    .AsTracking()
                    .FirstOrDefaultAsync();

                if (supplier is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                supplier.AuditDeleteUser = authenticatedUserId;
                supplier.AuditDeleteDate = DateTime.Now;
                supplier.Status = false;

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
