using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Action;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class ActionProjection
    {
        public static Expression<Func<ActionEntity, ActionReadModel>> ToSummary =>
            a => new ActionReadModel
            {
                Id = a.Id,
                ActionName = a.ActionName,
                IsActive = a.IsActive
            };
    }
}
