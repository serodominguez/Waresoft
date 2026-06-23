using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Supplier;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class SupplierQueryRepository : ISupplierQueryRepository
    {
        private readonly DbContextSystem _context;

        public SupplierQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<SupplierReadModel> GetSuppliersListQueryable()
        {
            return _context.Supplier
                .AsNoTracking()
                .Where(r => r.AuditDeleteUser == null && r.AuditDeleteDate == null)
                .Select(SupplierProjection.ToSummary);
        }

        public IQueryable<SupplierReadModel> GetSupplierByIdQueryable(int supplierId)
        {
            return _context.Supplier
                .AsNoTracking()
                .Where(r => r.Id == supplierId)
                .Select(SupplierProjection.ToSummary);
        }

        public IQueryable<SupplierSelectReadModel> GetSuppliersSelectQueryable()
        {
            return _context.Supplier
                .AsNoTracking()
                .Where(r => r.AuditDeleteUser == null && r.AuditDeleteDate == null)
                .Select(SupplierProjection.ToSelect);
        }
    }
}
