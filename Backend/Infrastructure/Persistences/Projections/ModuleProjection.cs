using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Module;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class ModuleProjection
    {
        public static Expression<Func<ModuleEntity, ModuleReadModel>> ToSummary =>
            m => new ModuleReadModel
            {
                Id = m.Id,
                ModuleName = m.ModuleName,
                AuditCreateDate = m.AuditCreateDate,
                Status = m.Status
            };
    }
}
