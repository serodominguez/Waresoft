using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ProductRepository : GenericRepository<ProductEntity>, IProductRepository
    {
        private readonly DbContextSystem _context;

        public ProductRepository(DbContextSystem context) : base(context)
        {
            _context = context;
        }

        public IQueryable<ProductEntity> GetProductsQueryable()
        {
            return _context.Product
                   .AsNoTracking()
                   .Where(p => p.AuditDeleteUser == null && p.AuditDeleteDate == null)
                   .Select(p => new ProductEntity
                   {
                       Id = p.Id,
                       Code = p.Code,
                       Description = p.Description,
                       Material = p.Material,
                       Color = p.Color,
                       UnitMeasure = p.UnitMeasure,
                       IdBrand = p.IdBrand,
                       Brand = new BrandEntity
                       {
                           BrandName = p.Brand.BrandName
                       },
                       IdCategory = p.IdCategory,
                       Category = new CategoryEntity
                       {
                           CategoryName = p.Category.CategoryName
                       },
                       AuditCreateDate = p.AuditCreateDate,
                       Status = p.Status
                   });
        }
     }
}
