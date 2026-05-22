using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Supplier;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class SupplierProjection
    {
        public static Expression<Func<SupplierEntity, SupplierReadModel>> ToSummary =>
            s => new SupplierReadModel
            {
                Id = s.Id,
                CompanyName = s.CompanyName,
                Contact = s.Contact,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                AuditCreateDate = s.AuditCreateDate,
                Status = s.Status
            };

        public static Expression<Func<SupplierEntity, SupplierSelectReadModel>> ToSelect =>
            s => new SupplierSelectReadModel
            {
                Id = s.Id,
                CompanyName = s.CompanyName,
                Status = s.Status,
            };
    }
}
