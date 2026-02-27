using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAllAsQueryable();
        IQueryable<T> GetAllActiveQueryable();
        IQueryable<T> GetSelectQueryable();
        IQueryable<T> GetByIdAsQueryable(int id);
        IQueryable<T> GetEntityQuery(Expression<Func<T, bool>>? filter = null);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        
    }
}
