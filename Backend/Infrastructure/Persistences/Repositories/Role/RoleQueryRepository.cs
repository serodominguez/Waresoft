using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Role;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class RoleQueryRepository : IRoleQueryRepository
    {
        private readonly DbContextSystem _context;

        public RoleQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<RoleReadModel> GetRolesListQueryable()
        {
            return _context.Role
                .AsNoTracking()
                .Where(r => r.AuditDeleteUser == null && r.AuditDeleteDate == null)
                .Select(RoleProjection.ToSummary);
        }

        public IQueryable<RoleReadModel> GetRoleByIdQueryable(int roleId)
        {
            return _context.Role
                .AsNoTracking()
                .Where(r => r.Id == roleId)
                .Select(RoleProjection.ToSummary);
        }

        public IQueryable<RoleSelectReadModel> GetRolesSelectQueryable()
        {
            return _context.Role
                .AsNoTracking()
                .Where(r => r.AuditDeleteUser == null && r.AuditDeleteDate == null)
                .Select(RoleProjection.ToSelect);
        }
    }
}
