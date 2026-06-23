using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Category;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class CategoryQueryRepository : ICategoryQueryRepository
    {
        private readonly DbContextSystem _context;

        public CategoryQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<CategoryReadModel> GetCategoriesListQueryable()
        {
            return _context.Category
                .AsNoTracking()
                .Where(c => c.AuditDeleteUser == null && c.AuditDeleteDate == null)
                .Select(CategoryProjection.ToSummary);
        }

        public IQueryable<CategoryReadModel> GetCategoryByIdQueryable(int categoryId)
        {
            return _context.Category
                .AsNoTracking()
                .Where(c => c.Id == categoryId)
                .Select(CategoryProjection.ToSummary);
        }

        public IQueryable<CategorySelectReadModel> GetCategoriesSelectQueryable()
        {
            return _context.Category
                .AsNoTracking()
                .Where(c => c.AuditDeleteUser == null && c.AuditDeleteDate == null)
                .Select(CategoryProjection.ToSelect);
        }
    }
}
