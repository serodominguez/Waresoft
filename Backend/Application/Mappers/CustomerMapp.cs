using Application.Dtos.Request.Customer;
using Application.Dtos.Response.Customer;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class CustomerMapp
    {
        public static CustomerEntity CustomersMapping(CustomerRequestDto dto)
        {
            return new CustomerEntity
            {
                Names = dto.Names.NormalizeString(),
                LastNames = dto.LastNames.NormalizeString(),
                IdentificationNumber = dto.IdentificationNumber.NormalizeString(),
                PhoneNumber = dto.PhoneNumber
            };
        }

        public static CustomerResponseDto CustomersResponseDtoMapping(CustomerEntity entity)
        {
            return new CustomerResponseDto
            {
                IdCustomer = entity.Id,
                Names = entity.Names.ToSentenceCase(),
                LastNames = entity.LastNames.ToSentenceCase(),
                IdentificationNumber = entity.IdentificationNumber.ToSentenceCase(),
                PhoneNumber = entity.PhoneNumber,
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusCustomer = ((States)(entity.Status ? 1 : 0)).ToString()
            };
        }
    }
}
