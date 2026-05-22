using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Customer;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class CustomerProjection
    {
        public static Expression<Func<CustomerEntity, CustomerReadModel>> ToSummary =>
            c => new CustomerReadModel
            {
                Id = c.Id,
                Names = c.Names,
                LastNames = c.LastNames,
                IdentificationNumber = c.IdentificationNumber,
                PhoneNumber = c.PhoneNumber,
                AuditCreateDate = c.AuditCreateDate,
                Status = c.Status
            };

        public static Expression<Func<CustomerEntity, CustomerSelectReadModel>> ToSelect =>
            c => new CustomerSelectReadModel
            {
                Id = c.Id,
                Names = c.Names,
                LastNames = c.LastNames,
                Status = c.Status
            };
    }
}
