using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Store)
                .Where(u => u.AuditDeleteUser == null && u.AuditDeleteDate == null);
        }

        public async Task<bool> EditUserAsync(int authenticatedUserId, UserEntity user, bool? updatePassword)
        {
            user.AuditUpdateUser = authenticatedUserId;
            user.AuditUpdateDate = DateTime.Now;

            if (updatePassword == true)
            {
                _context.Update(user);
                _context.Entry(user).Property(x => x.Status).IsModified = false;
                _context.Entry(user).Property(x => x.AuditCreateUser).IsModified = false;
                _context.Entry(user).Property(x => x.AuditCreateDate).IsModified = false;
            }
            else
            {
                _context.Update(user);
                _context.Entry(user).Property(x => x.PasswordHash).IsModified = false;
                _context.Entry(user).Property(x => x.PasswordSalt).IsModified = false;
                _context.Entry(user).Property(x => x.Status).IsModified = false;
                _context.Entry(user).Property(x => x.AuditCreateUser).IsModified = false;
                _context.Entry(user).Property(x => x.AuditCreateDate).IsModified = false;
            }

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
