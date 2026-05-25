using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Permission;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class PermissionProjection
    {
        public static Expression<Func<PermissionEntity, PermissionReadModel>> ToSummary =>
            p => new PermissionReadModel
            {
                Id = p.Id,
                IdRole = p.IdRole,
                IsActive = p.IsActive,
                IdModule = p.IdModule,
                ModuleName = p.Module.ModuleName,
                IsActiveModule = p.Module.IsActive,
                IdAction = p.IdAction,
                ActionName = p.Action.ActionName,
                IsActiveAction = p.Action.IsActive
            };
    }
}
