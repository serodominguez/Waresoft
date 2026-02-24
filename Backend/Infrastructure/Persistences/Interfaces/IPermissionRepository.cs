using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IPermissionRepository : IGenericRepository<PermissionEntity>
    {
        Task<bool> GetPermissionsAsync(int roleId, string moduleName, string actionName);
        Task<IEnumerable<PermissionEntity>> PermissionsByRoleAsync(int roleId);
        Task<IEnumerable<PermissionEntity>> GetByIdsAsync(List<int> permissionId);
        Task<IEnumerable<PermissionEntity>> GetByIdsForUpdateAsync(List<int> permissionId);
    }
}
