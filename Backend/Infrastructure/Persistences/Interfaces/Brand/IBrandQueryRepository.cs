using Infrastructure.Persistences.ReadModels.Brand;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IBrandQueryRepository
    {
        IQueryable<BrandReadModel> GetBrandsListQueryable();
        IQueryable<BrandReadModel> GetBrandByIdQueryable(int brandId);
        IQueryable<BrandSelectReadModel> GetBrandsSelectQueryable();
    }
}
