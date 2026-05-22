using Application.Dtos.Request.Supplier;
using Application.Dtos.Response.Category;
using Application.Dtos.Response.Supplier;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Supplier;
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
                Email = dto.Email,
            };
        }
        public static SupplierResponseDto SuppliersResponseDtoMapping(SupplierReadModel model)
        {
            return new SupplierResponseDto
            {
                IdSupplier = model.Id,
                CompanyName = model.CompanyName.ToTitleCase(),
                Contact = model.Contact.ToSentenceCase(),
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusSupplier = ((States)(model.Status ? 1 : 0)).ToString()
            };
        }

        public static SupplierSelectResponseDto SuppliersSelectResponseDtoMapping(SupplierSelectReadModel model)
        {
            return new SupplierSelectResponseDto
            {
                IdSupplier = model.Id,
                CompanyName = model.CompanyName.ToTitleCase()
            };
        }
    }
}
