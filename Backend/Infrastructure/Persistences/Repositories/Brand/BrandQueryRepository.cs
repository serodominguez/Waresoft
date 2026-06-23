using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Brand;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class BrandQueryRepository : IBrandQueryRepository
    {
        private readonly DbContextSystem _context;

        public BrandQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<BrandReadModel> GetBrandsListQueryable()
        {
            return _context.Brand
                .AsNoTracking()
                .Where(b => b.AuditDeleteUser == null && b.AuditDeleteDate == null)
                .Select(BrandProjection.ToSummary);
        }

        public IQueryable<BrandReadModel> GetBrandByIdQueryable(int brandId)
        {
            return _context.Brand
                .AsNoTracking()
                .Where(b => b.Id == brandId)
                .Select(BrandProjection.ToSummary);
        }

        public IQueryable<BrandSelectReadModel> GetBrandsSelectQueryable()
        {
            return _context.Brand
                .AsNoTracking()
                .Where(b => b.AuditDeleteUser == null && b.AuditDeleteDate == null)
                .Select(BrandProjection.ToSelect);
        }
    }
}
