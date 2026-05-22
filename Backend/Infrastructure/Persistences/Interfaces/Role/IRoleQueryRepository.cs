using Infrastructure.Persistences.ReadModels.Role;

namespace Infrastructure.Persistences.Interfaces.Role
{
    public interface IRoleQueryRepository
    {
        IQueryable<RoleReadModel> GetRolesListQueryable();
        IQueryable<RoleReadModel> GetRoleByIdQueryable(int roleId);
        IQueryable<RoleSelectReadModel> GetRolesSelectQueryable();
    }
}
