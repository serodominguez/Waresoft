using Infrastructure.Persistences.ReadModels.Product;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IProductQueryRepository
    {
        IQueryable<ProductReadModel> GetProductsQueryable();
        IQueryable<ProductReadModel> GetProductByIdQueryable(int productId);
        IQueryable<ProductSelectReadModel> GetProductsSelectQueryable();
    }
}
