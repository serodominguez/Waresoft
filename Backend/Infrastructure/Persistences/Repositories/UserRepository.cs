using Dapper;
using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class UserRepository : GenericRepository<UserEntity>, IUserRepository
    {
        private readonly DbContextSystem _context;
        private readonly string _connectionString;

        public UserRepository(DbContextSystem context) : base(context)
        {
            _context = context;
            _connectionString = context.Database.GetConnectionString()!;
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
                    AuditCreateDate = u.AuditCreateDate,
                    Role = new RoleEntity
                    {
                        Id = u.Role.Id,
                        RoleName = u.Role.RoleName
                    },
                    Store = new StoreEntity
                    {
                        Id = u.Store.Id,
                        StoreName = u.Store.StoreName,
                        Type = u.Store.Type
                    }
                });
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(string userName, bool status)
        {
            const string sql = @"
                SELECT 
                    u.IdUser as Id, u.UserName, u.PasswordHash, u.PasswordSalt, u.Names, u.LastNames, 
                    u.IdentificationNumber,u.PhoneNumber, u.IdRole, u.IdStore, u.Status, u.AuditCreateDate,
                    r.IdRole as Id, r.RoleName,
                    s.IdStore as Id, s.StoreName, s.Type
                FROM USERS u
                INNER JOIN ROLES r ON r.IdRole = u.IdRole
                INNER JOIN STORES s ON s.IdStore = u.IdStore
                WHERE u.AuditDeleteUser IS NULL 
                  AND u.AuditDeleteDate IS NULL
                  AND u.UserName = @UserName
                  AND u.Status = @Status";

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<UserEntity, RoleEntity, StoreEntity, UserEntity>(
                sql,
                (user, role, store) =>
                {
                    user.Role = role;
                    user.Store = store;
                    return user;
                },
                new { UserName = userName, Status = status ? 1 : 0 },
                splitOn: "Id, Id"
            );

            return result.FirstOrDefault();
        }
    }
}
