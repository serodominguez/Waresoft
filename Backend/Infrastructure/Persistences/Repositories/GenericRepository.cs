using Domain.Entities;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public IQueryable<T> GetAllActiveQueryable()
        {
            return GetEntityQuery()
                .Where(x => x.AuditDeleteUser == null && x.AuditDeleteDate == null);
        }

        public IQueryable<T> GetSelectQueryable()
        {
            return _entity
                .Where(x => x.AuditDeleteUser == null && x.AuditDeleteDate == null);
        }

        public IQueryable<T> GetByIdAsQueryable(int id)
        {
            return _entity
                .Where(x => x.Id == id);
        }

        public IQueryable<T> GetEntityQuery(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _entity;
            if (filter != null) query = query.Where(filter);
            return query;
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
