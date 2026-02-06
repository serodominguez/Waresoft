using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IProductRepository : IGenericRepository<ProductEntity>
    {
        IQueryable<ProductEntity> GetProductsQueryable();
    }
}
