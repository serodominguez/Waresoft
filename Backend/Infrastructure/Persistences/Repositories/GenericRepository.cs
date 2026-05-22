using Domain.Entities;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _entity;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        public IQueryable<T> GetAllAsQueryable()
        {
            return _entity;
        }

        public IQueryable<T> GetByIdAsQueryable(int id)
        {
            return _entity
                .AsTracking()
                .Where(e => e.Id == id);
        }

        public IQueryable<T> GetListOfIdsAsQueryable(List<int> ids)
        {
            return _entity
                .Where(e => ids.Contains(e.Id));
        }

        public async Task AddAsync(T entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.AddRangeAsync(entities);
        }
    }
}
