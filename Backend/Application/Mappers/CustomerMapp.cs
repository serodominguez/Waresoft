using Application.Dtos.Request.Customer;
using Application.Dtos.Response.Customer;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Customer;
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

        public static CustomerResponseDto CustomersResponseDtoMapping(CustomerReadModel model)
        {
            return new CustomerResponseDto
            {
                IdCustomer = model.Id,
                Names = model.Names.ToSentenceCase(),
                LastNames = model.LastNames.ToSentenceCase(),
                IdentificationNumber = model.IdentificationNumber.ToSentenceCase(),
                PhoneNumber = model.PhoneNumber,
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusCustomer = ((States)(model.IsActive ? 1 : 0)).ToString()
            };
        }

        public static CustomerStatsResponseDto CustomerStatsResponseDtoMapping(CustomerStatsReadModel model, decimal percentageChange, bool isPositive)
        {
            return new CustomerStatsResponseDto
            {
                TotalActive = model.TotalActive,
                PercentageChange = percentageChange,
                IsPositive = isPositive
            };
        }
    }
}
