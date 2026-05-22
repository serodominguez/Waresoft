using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Module;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Module;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.Module
{
    public class ModuleQueryRepository : IModuleQueryRepository
    {
        private readonly DbContextSystem _context;

        public ModuleQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<ModuleReadModel> GetModuleListQueryable()
        {
            return _context.Module
                .AsNoTracking()
                .Where(m => m.AuditDeleteUser == null && m.AuditDeleteDate == null)
                .Select(ModuleProjection.ToSummary);
        }

        public IQueryable<ModuleReadModel> GetModuleByIdQueryable(int customerId)
        {
            return _context.Module
                .AsNoTracking()
                .Where(m => m.Id == customerId)
                .Select(ModuleProjection.ToSummary);
        }
    }
}
