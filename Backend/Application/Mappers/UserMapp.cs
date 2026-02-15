using Application.Dtos.Request.User;
using Application.Dtos.Response.User;
using Domain.Entities;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class UserMapp
    {
        public static UserEntity UsersMapping(UserRequestDto dto)
        {
            return new UserEntity
            {
                UserName = dto.UserName,
                Names = dto.Names.NormalizeString(),
                LastNames = dto.LastNames.NormalizeString(),
                IdentificationNumber = dto.IdentificationNumber.NormalizeString(),
                PhoneNumber = dto.PhoneNumber,
                IdRole = dto.IdRole,
                IdStore = dto.IdStore
            };
        }

        public static UserResponseDto UsersResponseDtoMapping(UserEntity entity)
        {
            return new UserResponseDto
            {
                IdUser = entity.Id,
                UserName = entity.UserName,
                PasswordHash = entity.PasswordHash,
                Names = entity.Names.ToSentenceCase(),
                LastNames = entity.LastNames.ToSentenceCase(),
                IdentificationNumber = entity.IdentificationNumber.ToTitleCase(),
                PhoneNumber = entity.PhoneNumber,
                IdRole = entity.IdRole,
                RoleName = entity.Role?.RoleName.ToSentenceCase(),
                IdStore = entity.IdStore,
                StoreName = entity.Store?.StoreName.ToTitleCase(),
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusUser = ((States)(entity.Status ? 1 : 0)).ToString()
            };
        }

        public static UserSelectResponseDto UsersSelectResponseDtoMapping(UserEntity entity)
        {
            return new UserSelectResponseDto
            {
                IdUser = entity.Id,
                UserName = (entity.Names + " " + entity.LastNames).ToTitleCase(),
            };
        }
    }
}
