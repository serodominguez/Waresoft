using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAllQueryable();
        Task<IEnumerable<T>> GetSelectAsync();
        Task<T?> GetByIdAsync(int id);
        Task<bool> RegisterAsync(T entity);
        Task<bool> EditAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> RemoveAsync(T entity);
        IQueryable<T> GetEntityQuery(Expression<Func<T, bool>>? filter = null);
    }
}
