using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.User;
using Application.Dtos.Response.User;
using Application.Interfaces;
using Application.Mappers;
using Application.Security;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;
using Utilities.Static;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurity _security;
        private readonly IValidator<UserRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public UserService(IUnitOfWork unitOfWork, ISecurity security, IValidator<UserRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _security = security;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<UserResponseDto>>> ListUsers(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<UserResponseDto>>();

            try
            {
                var users = _unitOfWork.User.GetUsersQueryable();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            users = users.Where(x => x.UserName!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            users = users.Where(x => x.Names!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            users = users.Where(x => x.LastNames!.Contains(filters.TextFilter));
                            break;
                        case 4:
                            users = users.Where(x => x.Store!.StoreName!.Contains(filters.TextFilter));
                            break;
                        case 5:
                            users = users.Where(x => x.Role!.RoleName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    users = users.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);
                    users = users.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }

                response.TotalRecords = await users.CountAsync();

                filters.Sort ??= "Id";
                var items = await _orderingQuery.Ordering(filters, users, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(UserMapp.UsersResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<UserSelectResponseDto>>> ListSelectUsers()
        {
            var response = new BaseResponse<IEnumerable<UserSelectResponseDto>>();

            try
            {
                var users = (await _unitOfWork.User.GetSelectAsync());

                if (users is not null && users.Any())
                {
                    response.Data = users.Select(UserMapp.UsersSelectResponseDtoMapping);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<UserResponseDto>> UserById(int userId)
        {
            var response = new BaseResponse<UserResponseDto>();

            try
            {
                var user = await _unitOfWork.User.GetByIdAsync(userId);

                if (user is not null)
                {
                    response.Data = UserMapp.UsersResponseDtoMapping(user);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterUser(int authenticatedUserId, UserRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var validationResult = await _validator.ValidateAsync(requestDto);

                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    response.Errors = validationResult.Errors;
                    return response;
                }

                _security.GeneratePasswordHash(requestDto.Password!, out byte[] passwordHash, out byte[] passwordSalt);

                var user = UserMapp.UsersMapping(requestDto);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.AuditCreateUser = authenticatedUserId;
                user.AuditCreateDate = DateTime.Now;
                user.Status = true;

                await _unitOfWork.User.AddAsync(user);

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_SAVE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditUser(int authenticatedUserId, int userId, UserRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var validationResult = await _validator.ValidateAsync(requestDto);

                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    response.Errors = validationResult.Errors;
                    return response;
                }

                var user = await _unitOfWork.User.GetByIdForUpdateAsync(userId);

                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                user.UserName = requestDto.UserName;
                user.Names = requestDto.Names.NormalizeString();
                user.LastNames = requestDto.LastNames.NormalizeString();
                user.IdentificationNumber = requestDto.IdentificationNumber.NormalizeString();
                user.PhoneNumber = requestDto.PhoneNumber;
                user.IdRole = requestDto.IdRole;
                user.IdStore = requestDto.IdStore;

                if (requestDto.UpdatePassword == true)
                {
                    _security.GeneratePasswordHash(requestDto.Password!, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }

                user.AuditUpdateUser = authenticatedUserId;
                user.AuditUpdateDate = DateTime.Now;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EnableUser(int authenticatedUserId, int userId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var user = await _unitOfWork.User.GetByIdForUpdateAsync(userId);

                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                user.AuditCreateUser = authenticatedUserId;
                user.AuditCreateDate = DateTime.Now;
                user.Status = true;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DisableUser(int authenticatedUserId, int userId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var user = await _unitOfWork.User.GetByIdForUpdateAsync(userId);

                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                user.AuditUpdateUser = authenticatedUserId;
                user.AuditUpdateDate = DateTime.Now;
                user.Status = true;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveUser(int authenticatedUserId, int userId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var user = await _unitOfWork.User.GetByIdForUpdateAsync(userId);

                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                user.AuditDeleteUser = authenticatedUserId;
                user.AuditDeleteDate = DateTime.Now;
                user.Status = false;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }
    }
}
