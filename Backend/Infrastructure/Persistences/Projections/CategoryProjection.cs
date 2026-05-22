using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Category;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class CategoryProjection
    {
        public static Expression<Func<CategoryEntity, CategoryReadModel>> ToSummary =>
            c => new CategoryReadModel
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
                AuditCreateDate = c.AuditCreateDate,
                Status = c.Status
            };

        public static Expression<Func<CategoryEntity, CategorySelectReadModel>> ToSelect =>
            c => new CategorySelectReadModel
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Status = c.Status,
            };
    }
}
