using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Permission;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Permission;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.Permission
{
    public class PermissionQueryRepository : IPermissionQueryRepository
    {
        private readonly DbContextSystem _context;

        public PermissionQueryRepository(DbContextSystem context) 
        {
            _context = context;
        }

        public async Task<bool> GetPermissionsAsync(int roleId, string moduleName, string actionName)
        {
            return await _context.Permission
                .AsNoTracking()
                .AnyAsync(p =>
                    p.IdRole == roleId &&
                    p.Module.ModuleName == moduleName &&
                    p.Action.ActionName == actionName &&
                    p.IsActive &&
                    p.Module.IsActive &&
                    p.Action.IsActive
                );
        }

        public IQueryable<PermissionReadModel> PermissionsByRoleAsQueryable(int roleId)
        {
            return _context.Permission
                .AsNoTracking()
                .Where(p => p.IdRole == roleId)
                .Select(PermissionProjection.ToSummary);
        }

    }
}
