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
                Status = p.Status,
                IdModule = p.IdModule,
                ModuleName = p.Module.ModuleName,
                StatusModule = p.Module.Status,
                IdAction = p.IdAction,
                ActionName = p.Action.ActionName,
                StatusAction = p.Action.Status
            };
    }
}
