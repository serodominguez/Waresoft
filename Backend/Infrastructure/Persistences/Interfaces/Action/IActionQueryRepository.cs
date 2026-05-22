using Infrastructure.Persistences.ReadModels.Action;

namespace Infrastructure.Persistences.Interfaces.Action
{
    public interface IActionQueryRepository
    {
        IQueryable<ActionReadModel> GetActionsListQueryable();
    }
}
