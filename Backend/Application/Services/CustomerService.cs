using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Customer;
using Application.Dtos.Response.Customer;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CustomerRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public CustomerService(IUnitOfWork unitOfWork, IValidator<CustomerRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<CustomerResponseDto>>> ListCustomers(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<CustomerResponseDto>>();

            try
            {
                var customers = _unitOfWork.Customer.GetAllQueryable();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            customers = customers.Where(x => x.Names!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            customers = customers.Where(x => x.LastNames!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            customers = customers.Where(x => x.IdentificationNumber!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    customers = customers.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                    customers = customers.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }

                response.TotalRecords = await customers.CountAsync();

                filters.Sort ??= "Id";
                var items = await _orderingQuery.Ordering(filters, customers, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(CustomerMapp.CustomersResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<CustomerResponseDto>> CustomerById(int customerId)
        {
            var response = new BaseResponse<CustomerResponseDto>();

            try
            {
                var customer = await _unitOfWork.Customer.GetByIdAsync(customerId);

                if (customer is not null)
                {
                    response.Data = CustomerMapp.CustomersResponseDtoMapping(customer);
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

        public async Task<BaseResponse<bool>> RegisterCustomer(int authenticatedUserId, CustomerRequestDto requestDto)
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

                var customer = CustomerMapp.CustomersMapping(requestDto);
                customer.AuditCreateUser = authenticatedUserId;
                customer.AuditCreateDate = DateTime.Now;
                customer.Status = true;

                await _unitOfWork.Customer.AddAsync(customer);

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

        public async Task<BaseResponse<bool>> EditCustomer(int authenticatedUserId, int customerId, CustomerRequestDto requestDto)
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

                var customer = await _unitOfWork.Customer.GetByIdForUpdateAsync(customerId);

                if (customer is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                customer.Names = requestDto.Names.NormalizeString();
                customer.LastNames = requestDto.LastNames.NormalizeString();
                customer.IdentificationNumber = requestDto.IdentificationNumber.NormalizeString();
                customer.PhoneNumber = requestDto.PhoneNumber;
                customer.AuditUpdateUser = authenticatedUserId;
                customer.AuditUpdateDate = DateTime.Now;

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

        public async Task<BaseResponse<bool>> EnableCustomer(int authenticatedUserId, int customerId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var customer = await _unitOfWork.Customer.GetByIdForUpdateAsync(customerId);

                if (customer is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                customer.AuditUpdateUser = authenticatedUserId;
                customer.AuditUpdateDate = DateTime.Now;
                customer.Status = true;

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

        public async Task<BaseResponse<bool>> DisableCustomer(int authenticatedUserId, int customerId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var customer = await _unitOfWork.Customer.GetByIdForUpdateAsync(customerId);

                if (customer is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                customer.AuditUpdateUser = authenticatedUserId;
                customer.AuditUpdateDate = DateTime.Now;
                customer.Status = false;

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

        public async Task<BaseResponse<bool>> RemoveCustomer(int authenticatedUserId, int customerId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var customer = await _unitOfWork.Customer.GetByIdForUpdateAsync(customerId);

                if (customer is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                customer.AuditDeleteUser = authenticatedUserId;
                customer.AuditDeleteDate = DateTime.Now;
                customer.Status = false;

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
