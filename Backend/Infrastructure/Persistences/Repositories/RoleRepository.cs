using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class RoleRepository : GenericRepository<RoleEntity>, IRoleRepository
    {
        private readonly DbContextSystem _context;

        public RoleRepository(DbContextSystem context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RoleEntity>> GetRolesAsync()
        {
            return await _context.Role
                .AsNoTracking()
                .Where(r => r.AuditDeleteUser == null && r.AuditDeleteDate == null)  //&& r.Id != 1)
                .ToListAsync();
        }

        public async Task<RoleEntity> RegisterRoleAsync(RoleEntity role)
        {
            await _context.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;

        }
    }
}
