using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Store;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class StoreQueryRepository : IStoreQueryRepository
    {
        private readonly DbContextSystem _context;

        public StoreQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<StoreReadModel> GetStoresListQueryable()
        {
            return _context.Store
                .AsNoTracking()
                .Where(r => r.AuditDeleteUser == null && r.AuditDeleteDate == null)
                .Select(StoreProjection.ToSummary);
        }

        public IQueryable<StoreReadModel> GetStoreByIdQueryable(int roleId)
        {
            return _context.Store
                .AsNoTracking()
                .Where(r => r.Id == roleId)
                .Select(StoreProjection.ToSummary);
        }

        public IQueryable<StoreSelectReadModel> GetStoresSelectQueryable()
        {
            return _context.Store
                .AsNoTracking()
                .Where(r => r.AuditDeleteUser == null && r.AuditDeleteDate == null)
                .Select(StoreProjection.ToSelect);
        }
    }
}
