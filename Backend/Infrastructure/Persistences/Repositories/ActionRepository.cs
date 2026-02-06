using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ActionRepository : GenericRepository<ActionEntity>, IActionRepository
    {
        private readonly DbContextSystem _context;

        public ActionRepository(DbContextSystem context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ActionEntity>> GetActionsAsync()
        {
            return await _context.Action
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
