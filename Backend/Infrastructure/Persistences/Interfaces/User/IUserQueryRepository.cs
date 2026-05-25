using Infrastructure.Persistences.ReadModels.User;

namespace Infrastructure.Persistences.Interfaces.User
{
    public interface IUserQueryRepository
    {
        IQueryable<UserReadModel> GetUsersListQueryable();
        IQueryable<UserReadModel> GetUserByIdQueryable(int userId);
        IQueryable<UserSelectReadModel> GetUsersSelectQueryable();
        Task<UserAccountReadModel?> GetUserAccountAsync(string userName, bool IsActive);
    }
}
