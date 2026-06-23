using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Action;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ActionQueryRepository : IActionQueryRepository
    {
        private readonly DbContextSystem _context;

        public ActionQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<ActionReadModel> GetActionsListQueryable()
        {
            return _context.Action
                .AsNoTracking()
                .Where(a => a.AuditDeleteUser == null && a.AuditDeleteDate == null)
                .Select(ActionProjection.ToSummary);
        }
    }
}
