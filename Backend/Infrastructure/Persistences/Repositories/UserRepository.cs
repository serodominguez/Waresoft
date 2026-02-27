using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class UserRepository : GenericRepository<UserEntity>, IUserRepository
    {
        private readonly DbContextSystem _context;

        public UserRepository(DbContextSystem context) : base(context)
        {
            _context = context;
        }

        public IQueryable<UserEntity> GetUsersQueryable()
        {
            return _context.User
                .Where(u => u.AuditDeleteUser == null && u.AuditDeleteDate == null)
                .Select(u => new UserEntity
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Names = u.Names,
                    LastNames = u.LastNames,
                    IdentificationNumber = u.IdentificationNumber,
                    PhoneNumber = u.PhoneNumber,
                    PasswordHash = u.PasswordHash,
                    PasswordSalt = u.PasswordSalt,
                    IdRole = u.IdRole,
                    IdStore = u.IdStore,
                    Status = u.Status,
                    Role = new RoleEntity
                    {
                        Id = u.Role.Id,
                        RoleName = u.Role.RoleName
                    },
                    Store = new StoreEntity
                    {
                        Id = u.Store.Id,
                        StoreName = u.Store.StoreName
                    }
                });
        }
    }
}
