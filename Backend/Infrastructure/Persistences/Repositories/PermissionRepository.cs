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
            return await _context.Permission
                .AsNoTracking()
                .Include(p => p.Module)
                .Include(p => p.Action)
                .Where(p => p.IdRole == roleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PermissionEntity>> GetByIdsAsync(List<int> permissionIds)
        {
            return await _context.Permission
                 .AsNoTracking()
                 .Where(p => permissionIds.Contains(p.Id))
                 .ToListAsync();
        }

        public async Task<bool> RegisterPermissionsAsync(List<PermissionEntity> permissions)
        {
            await _context.Permission.AddRangeAsync(permissions);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> UpdatePermissionsRangeAsync(List<PermissionEntity> permissions)
        {
            foreach (var permission in permissions)
            {
                _context.Update(permission);
                _context.Entry(permission).Property(x => x.IdRole).IsModified = false;
                _context.Entry(permission).Property(x => x.IdModule).IsModified = false;
                _context.Entry(permission).Property(x => x.IdAction).IsModified = false;
                _context.Entry(permission).Property(x => x.AuditCreateUser).IsModified = false;
                _context.Entry(permission).Property(x => x.AuditCreateDate).IsModified = false;
            }

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
