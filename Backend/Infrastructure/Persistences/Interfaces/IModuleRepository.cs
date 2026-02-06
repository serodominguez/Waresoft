using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IModuleRepository : IGenericRepository<ModuleEntity>
    {
        Task<List<ModuleEntity>> GetModulesAsync();
        public Task<ModuleEntity> RegisterModuleAsync(ModuleEntity module);
    }
}
