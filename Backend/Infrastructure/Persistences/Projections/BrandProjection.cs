using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Brand;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class BrandProjection
    {
        public static Expression<Func<BrandEntity, BrandReadModel>> ToSummary =>
            b => new BrandReadModel
            {
                Id = b.Id,
                BrandName = b.BrandName,
                AuditCreateDate = b.AuditCreateDate,
                Status = b.Status
            };

        public static Expression<Func<BrandEntity, BrandSelectReadModel>> ToSelect =>
            b => new BrandSelectReadModel
            {
                Id = b.Id,
                BrandName = b.BrandName,
                Status = b.Status,
            };
    }
}
