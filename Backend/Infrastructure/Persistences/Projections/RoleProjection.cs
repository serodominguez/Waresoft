using Domain.Entities;
using Infrastructure.Persistences.ReadModels.Role;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class RoleProjection
    {
        public static Expression<Func<RoleEntity, RoleReadModel>> ToSummary =>
            r => new RoleReadModel
            {
                Id = r.Id,
                RoleName = r.RoleName,
                AuditCreateDate = r.AuditCreateDate,
                Status = r.Status
            };

        public static Expression<Func<RoleEntity, RoleSelectReadModel>> ToSelect =>
            r => new RoleSelectReadModel
            {
                Id = r.Id,
                RoleName = r.RoleName,
                Status = r.Status,
            };
    }
}
