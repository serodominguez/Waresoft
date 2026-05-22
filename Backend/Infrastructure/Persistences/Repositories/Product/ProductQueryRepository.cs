using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Product;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Product;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.Product
{
    public class ProductQueryRepository : IProductQueryRepository
    {
        private readonly DbContextSystem _context;

        public ProductQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<ProductReadModel> GetProductsQueryable()
        {
            return _context.Product
                .AsTracking()
                .Where(p => p.AuditDeleteUser == null && p.AuditDeleteDate == null)
                .Select(ProductProjection.ToSummary);
        }

        public IQueryable<ProductReadModel> GetProductByIdQueryable(int productId)
        {
            return _context.Product
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(ProductProjection.ToSummary);
        }
    }
}
