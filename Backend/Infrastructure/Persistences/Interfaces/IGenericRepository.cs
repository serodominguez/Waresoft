using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAllAsQueryable();
        IQueryable<T> GetByIdAsQueryable(int id);
        IQueryable<T> GetListOfIdsAsQueryable(List<int> ids);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        
    }
}
