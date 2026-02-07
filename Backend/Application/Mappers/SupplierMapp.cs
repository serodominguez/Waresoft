using Application.Dtos.Request.Supplier;
using Application.Dtos.Response.Category;
using Application.Dtos.Response.Supplier;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class SupplierMapp
    {
        public static SupplierEntity SuppliersMapping(SupplierRequestDto dto)
        {
            return new SupplierEntity
            {
                CompanyName = dto.CompanyName.NormalizeString(),
                Contact = dto.Contact.NormalizeString(),
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email.NormalizeString(),
            };
        }
        public static SupplierResponseDto SuppliersResponseDtoMapping(SupplierEntity entity)
        {
            return new SupplierResponseDto
            {
                IdSupplier = entity.Id,
                CompanyName = entity.CompanyName.ToTitleCase(),
                Contact = entity.Contact.ToTitleCase(),
                Email = entity.Email?.ToLower(),
                PhoneNumber = entity.PhoneNumber,
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusSupplier = ((States)(entity.Status ? 1 : 0)).ToString()
            };
        }

        public static SupplierSelectResponseDto SuppliersSelectResponseDtoMapping(SupplierEntity entity)
        {
            return new SupplierSelectResponseDto
            {
                IdSupplier = entity.Id,
                CompanyName = entity.CompanyName.ToTitleCase()
            };
        }
    }
}
