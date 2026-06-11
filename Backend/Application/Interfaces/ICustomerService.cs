using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Customer;
using Application.Dtos.Response.Customer;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        Task<BaseResponse<IEnumerable<CustomerResponseDto>>> ListCustomers(BaseFiltersRequest filters);
        Task<BaseResponse<CustomerResponseDto>> CustomerById(int customerId);
        Task<BaseResponse<bool>> RegisterCustomer(int authenticatedUserId, CustomerRequestDto requestDto);
        Task<BaseResponse<bool>> EditCustomer(int authenticatedUserId, int customerId, CustomerRequestDto requestDto);
        Task<BaseResponse<bool>> EnableCustomer(int authenticatedUserId, int customerId);
        Task<BaseResponse<bool>> DisableCustomer(int authenticatedUserId, int customerId);
        Task<BaseResponse<bool>> RemoveCustomer(int authenticatedUserId, int customerId);
    }
}
