using Infrastructure.Persistences.ReadModels.Module;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IModuleQueryRepository
    {
        IQueryable<ModuleReadModel> GetModuleListQueryable();
        IQueryable<ModuleReadModel> GetModuleByIdQueryable(int customerId);
    }
}
