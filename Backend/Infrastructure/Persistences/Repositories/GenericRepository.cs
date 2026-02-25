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

        public IQueryable<T> GetAllQueryable()
        {
            return GetEntityQuery()
                .AsNoTracking()
                .Where(x => x.AuditDeleteUser == null && x.AuditDeleteDate == null);

        }

        public async Task<IEnumerable<T>> GetSelectAsync()
        {
            return await _entity
                    .AsNoTracking()
                    .Where(x => x.AuditDeleteUser == null && x.AuditDeleteDate == null && x.Status == true)
                    .ToListAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return  await _entity.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _entity.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<T?> GetByIdForUpdateAsync(int id)
        {
            return await _entity.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.AddRangeAsync(entities);
        }

        public async Task<bool> RegisterAsync(T entity)
        {
            await _context.AddAsync(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EditAsync(T entity)
        {
            _context.Update(entity);
            _context.Entry(entity).Property(x => x.Status).IsModified = false;
            _context.Entry(entity).Property(x => x.AuditCreateUser).IsModified = false;
            _context.Entry(entity).Property(x => x.AuditCreateDate).IsModified = false;
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public IQueryable<T> GetEntityQuery(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _entity;
            if (filter != null) query = query.Where(filter);
            return query;
        }
    }
}
