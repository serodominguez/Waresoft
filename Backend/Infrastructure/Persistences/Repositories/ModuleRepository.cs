using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ModuleRepository : GenericRepository<ModuleEntity>, IModuleRepository
    {
        private readonly DbContextSystem _context;

        public ModuleRepository(DbContextSystem context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ModuleEntity>> GetModulesAsync()
        {
            return await _context.Module
                .AsNoTracking()
                .Where(m => m.AuditDeleteUser == null && m.AuditDeleteDate == null)
                .ToListAsync();
        }

        public async Task<ModuleEntity> RegisterModuleAsync(ModuleEntity module)
        {
            await _context.AddAsync(module);
            await _context.SaveChangesAsync();
            return module;

        }
    }
}
