using Infrastructure.Persistences.ReadModels.Permission;

namespace Infrastructure.Persistences.Interfaces.Permission
{
    public interface IPermissionQueryRepository
    {
        Task<bool> GetPermissionsAsync(int roleId, string moduleName, string actionName);
        IQueryable<PermissionReadModel> PermissionsByRoleAsQueryable(int roleId);
    }
}
