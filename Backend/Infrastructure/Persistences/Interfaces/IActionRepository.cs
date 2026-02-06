using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IActionRepository : IGenericRepository<ActionEntity>
    {
        Task<List<ActionEntity>> GetActionsAsync();
    }
}
