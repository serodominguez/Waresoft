using Dapper;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.User;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.User;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.User
{
    public class UserQueryRepository : IUserQueryRepository
    {
        private readonly DbContextSystem _context;
        private readonly string _connectionString;

        public UserQueryRepository(DbContextSystem context)
        {
            _context = context;
            _connectionString = context.Database.GetConnectionString()!;
        }

        public IQueryable<UserReadModel> GetUsersListQueryable()
        {
            return _context.User
                .AsNoTracking()
                .Where(u => u.AuditDeleteUser == null && u.AuditDeleteDate == null)
                .Select(UserProjection.ToSummary);
        }

        public IQueryable<UserReadModel> GetUserByIdQueryable(int userId)
        {
            return _context.User
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(UserProjection.ToSummary);
        }

        public IQueryable<UserSelectReadModel> GetUsersSelectQueryable()
        {
            return _context.User
                .AsNoTracking()
                .Where(u => u.AuditDeleteUser == null && u.AuditDeleteDate == null)
                .Select(UserProjection.ToSelect);
        }

        public async Task<UserAccountReadModel?> GetUserAccountAsync(string userName, bool IsActive)
        {
            const string sql = @"
                                SELECT u.IdUser as Id, u.UserName, u.PasswordHash, u.PasswordSalt,
                                        u.IdRole, u.IdStore, r.RoleName, s.StoreName, s.Type
                                FROM USERS u
                                INNER JOIN ROLES r ON r.IdRole = u.IdRole
                                INNER JOIN STORES s ON s.IdStore = u.IdStore
                                WHERE u.AuditDeleteUser IS NULL 
                                AND u.AuditDeleteDate IS NULL
                                AND u.UserName = @UserName
                                AND u.IsActive = @IsActive";

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<UserAccountReadModel>(
                sql,
                new { UserName = userName, IsActive = IsActive ? 1 : 0 }
            );

            return result;
        }
    }
}
