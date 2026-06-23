using Infrastructure.Persistences.ReadModels.Role;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRoleQueryRepository
    {
        IQueryable<RoleReadModel> GetRolesListQueryable();
        IQueryable<RoleReadModel> GetRoleByIdQueryable(int roleId);
        IQueryable<RoleSelectReadModel> GetRolesSelectQueryable();
    }
}
