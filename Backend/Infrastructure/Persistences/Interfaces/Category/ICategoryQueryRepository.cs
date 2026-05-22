using Infrastructure.Persistences.ReadModels.Category;

namespace Infrastructure.Persistences.Interfaces.Category
{
    public interface ICategoryQueryRepository
    {
        IQueryable<CategoryReadModel> GetCategoriesListQueryable();
        IQueryable<CategoryReadModel> GetCategoryByIdQueryable(int categoryId);
        IQueryable<CategorySelectReadModel> GetCategoriesSelectQueryable();
    }
}
