using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUserRepository : IGenericRepository<UserEntity>
    {
        IQueryable<UserEntity> GetUsersQueryable();
        Task<bool> EditUserAsync(int authenticatedUserId, UserEntity user, bool? updatePassword);
    }
}
