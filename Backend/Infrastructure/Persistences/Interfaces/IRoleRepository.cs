using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRoleRepository : IGenericRepository<RoleEntity>
    {
        Task<List<RoleEntity>> GetRolesAsync();
        public Task<RoleEntity> RegisterRoleAsync(RoleEntity role);
    }
}
