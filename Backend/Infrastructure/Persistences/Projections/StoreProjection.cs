using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Store;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class StoreProjection
    {
        public static Expression<Func<StoreEntity, StoreReadModel>> ToSummary =>
            s => new StoreReadModel
            {
                Id = s.Id,
                StoreName = s.StoreName,
                Manager = s.Manager,
                Address = s.Address,
                PhoneNumber = s.PhoneNumber,
                City = s.City,
                Email = s.Email,
                ProfitMargin = s.ProfitMargin,
                Type = s.Type,
                AuditCreateDate = s.AuditCreateDate,
                Status = s.Status
            };

        public static Expression<Func<StoreEntity, StoreSelectReadModel>> ToSelect =>
            s => new StoreSelectReadModel
            {
                Id = s.Id,
                StoreName = s.StoreName,
                Status = s.Status,
            };
    }
}
