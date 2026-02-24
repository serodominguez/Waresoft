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

        public async Task<IEnumerable<PermissionEntity>> PermissionsByRoleAsync(int roleId)
        {
            var response = await _context.Permission
                .AsNoTracking()
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
                })
                .ToListAsync();
             return response;
        }

        public async Task<IEnumerable<PermissionEntity>> GetByIdsAsync(List<int> permissionIds)
        {
            return await _context.Permission
                .AsNoTracking()
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => new PermissionEntity
                {
                    Id = p.Id,
                    IdRole = p.IdRole,
                    IdModule = p.IdModule,
                    IdAction = p.IdAction,
                    Status = p.Status,
                    AuditUpdateUser = p.AuditUpdateUser,
                    AuditUpdateDate = p.AuditUpdateDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PermissionEntity>> GetByIdsForUpdateAsync(List<int> permissionIds)
        {
            return await _context.Permission
                .Where(p => permissionIds.Contains(p.Id))
                .ToListAsync();
        }
    }
}
