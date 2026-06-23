using Infrastructure.Persistences.ReadModels.Action;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IActionQueryRepository
    {
        IQueryable<ActionReadModel> GetActionsListQueryable();
    }
}
