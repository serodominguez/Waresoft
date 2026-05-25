using Domain.Entities;
using Infrastructure.Persistences.ReadModels.User;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Projections
{
    public static class UserProjection
    {
        public static Expression<Func<UserEntity, UserReadModel>> ToSummary =>
            u => new UserReadModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Names = u.Names,
                LastNames = u.LastNames,
                IdentificationNumber = u.IdentificationNumber,
                PhoneNumber = u.PhoneNumber,
                PasswordHash = u.PasswordHash,
                IdRole = u.IdRole,
                RoleName = u.Role.RoleName,
                IdStore = u.IdStore,
                StoreName = u.Store.StoreName,
                AuditCreateDate = u.AuditCreateDate,
                IsActive = u.IsActive
            };

        public static Expression<Func<UserEntity, UserSelectReadModel>> ToSelect =>
            u => new UserSelectReadModel
            {
                Id = u.Id,
                Names = u.Names, 
                LastNames = u.LastNames,
                IsActive = u.IsActive
            };
    }
}
