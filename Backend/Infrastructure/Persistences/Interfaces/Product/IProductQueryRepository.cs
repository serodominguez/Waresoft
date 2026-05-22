using Infrastructure.Persistences.ReadModels.Product;

namespace Infrastructure.Persistences.Interfaces.Product
{
    public interface IProductQueryRepository
    {
        IQueryable<ProductReadModel> GetProductsQueryable();
        IQueryable<ProductReadModel> GetProductByIdQueryable(int productId);
    }
}
