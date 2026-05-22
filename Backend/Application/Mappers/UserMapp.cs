using Application.Dtos.Request.User;
using Application.Dtos.Response.User;
using Domain.Entities;
using Infrastructure.Persistences.ReadModels.User;
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

        public static UserResponseDto UsersResponseDtoMapping(UserReadModel model)
        {
            return new UserResponseDto
            {
                IdUser = model.Id,
                UserName = model.UserName,
                PasswordHash = model.PasswordHash,
                Names = model.Names.ToTitleCase(),
                LastNames = model.LastNames.ToTitleCase(),
                IdentificationNumber = model.IdentificationNumber.ToTitleCase(),
                PhoneNumber = model.PhoneNumber,
                IdRole = model.IdRole,
                RoleName = model.RoleName.ToSentenceCase(),
                IdStore = model.IdStore,
                StoreName = model.StoreName.ToTitleCase(),
                AuditCreateDate = model.AuditCreateDate.HasValue ? model.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                StatusUser = ((States)(model.Status ? 1 : 0)).ToString()
                //StatusUser = ((States)((model.Status ?? false) ? 1 : 0)).ToString()
            };
        }

        public static UserSelectResponseDto UsersSelectResponseDtoMapping(UserSelectReadModel model)
        {
            return new UserSelectResponseDto
            {
                IdUser = model.Id,
                UserName = (model.Names + " " + model.LastNames).ToTitleCase(),
            };
        }
    }
}
