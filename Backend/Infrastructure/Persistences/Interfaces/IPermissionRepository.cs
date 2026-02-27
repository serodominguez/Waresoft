using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IPermissionRepository : IGenericRepository<PermissionEntity>
    {
        Task<bool> GetPermissionsAsync(int roleId, string moduleName, string actionName);
        IQueryable<PermissionEntity> PermissionsByRoleAsQueryable(int roleId);
        IQueryable<PermissionEntity> GetByIdsAsQueryable(List<int> permissionIds);
    }
}
