using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class PermissionRepository : GenericRepository<PermissionEntity>, IPermissionRepository
    {
        private readonly DbContextSystem _context;

        public PermissionRepository(DbContextSystem context) : base(context)
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
                    p.Status &&
                    p.Module.Status &&
                    p.Action.Status
                );
        }

        public IQueryable<PermissionEntity> PermissionsByRoleAsQueryable(int roleId)
        {
            return _context.Permission
                .Where(p => p.IdRole == roleId)
                .Select(p => new PermissionEntity
                {
                    Id = p.Id,
                    IdRole = p.IdRole,
                    IdModule = p.IdModule,
                    IdAction = p.IdAction,
                    Status = p.Status,
                    Module = new ModuleEntity
                    {
                        Id = p.Module.Id,
                        ModuleName = p.Module.ModuleName,
                        Status = p.Module.Status
                    },
                    Action = new ActionEntity
                    {
                        Id = p.Action.Id,
                        ActionName = p.Action.ActionName,
                        Status = p.Action.Status
                    }
                });
        }

        public IQueryable<PermissionEntity> GetByIdsAsQueryable(List<int> permissionIds)
        {
            return _context.Permission
                .Where(p => permissionIds.Contains(p.Id));
        }
    }
}
