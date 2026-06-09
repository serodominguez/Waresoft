using Infrastructure.Persistences.ReadModels.Category;
using Infrastructure.Persistences.ReadModels.Customer;

namespace Infrastructure.Persistences.Interfaces.Category
{
    public interface ICategoryQueryRepository
    {
        IQueryable<CategoryReadModel> GetCategoriesListQueryable();
        IQueryable<CategoryReadModel> GetCategoryByIdQueryable(int categoryId);
        IQueryable<CategorySelectReadModel> GetCategoriesSelectQueryable();
    }
}
