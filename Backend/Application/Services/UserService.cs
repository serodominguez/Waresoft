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
        private readonly ISecurityApplication _security;
        private readonly IValidator<UserRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public UserService(IUnitOfWork unitOfWork,ISecurityApplication security, IValidator<UserRequestDto> validator, IOrderingQuery orderingQuery)
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
                var users = _unitOfWork.UserQuery.GetUsersListQueryable();

                if (filters.NumberFilter.HasValue && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    users = filters.NumberFilter switch
                    {
                        1 => users.Where(x => x.UserName!.Contains(filters.TextFilter)),
                        2 => users.Where(x => x.Names!.Contains(filters.TextFilter)),
                        3 => users.Where(x => x.LastNames!.Contains(filters.TextFilter)),
                        4 => users.Where(x => x.StoreName!.Contains(filters.TextFilter)),
                        5 => users.Where(x => x.RoleName!.Contains(filters.TextFilter)),
                        _ => users
                    };
                }

                if (filters.StateFilter.HasValue)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    users = users.Where(x => x.IsActive == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = DateTime.Parse(filters.StartDate).Date;
                    var endDate = DateTime.Parse(filters.EndDate).Date.AddDays(1);
                    users = users.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }

                response.TotalRecords = await users.CountAsync();

                filters.Sort ??= "Id";
                var orderedQuery = _orderingQuery.Ordering(filters, users, !(bool)filters.Download!);
                var items = await orderedQuery.ToListAsync();

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

        public async Task<BaseResponse<IEnumerable<UserSelectResponseDto>>> SelectListUsers()
        {
            var response = new BaseResponse<IEnumerable<UserSelectResponseDto>>();

            try
            {
                var users = await _unitOfWork.UserQuery.GetUsersSelectQueryable()
                    .Where(u => u.IsActive == true)
                    .ToListAsync();

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
                var user = await _unitOfWork.UserQuery.GetUserByIdQueryable(userId)
                    .FirstOrDefaultAsync();

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
                user.IsActive = true;

                await _unitOfWork.UserCommand.AddAsync(user);

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

                var user = await _unitOfWork.UserCommand.GetByIdAsQueryable(userId)
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                user.UserName = requestDto.UserName!;
                user.Names = requestDto.Names.NormalizeString()!;
                user.LastNames = requestDto.LastNames.NormalizeString()!;
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
                var user = await _unitOfWork.UserCommand.GetByIdAsQueryable(userId)
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                user.AuditUpdateUser = authenticatedUserId;
                user.AuditUpdateDate = DateTime.Now;
                user.IsActive = true;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
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
                var user = await _unitOfWork.UserCommand.GetByIdAsQueryable(userId)
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                user.AuditUpdateUser = authenticatedUserId;
                user.AuditUpdateDate = DateTime.Now;
                user.IsActive = false;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_INACTIVATE;
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
                var user = await _unitOfWork.UserCommand.GetByIdAsQueryable(userId)
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                user.AuditDeleteUser = authenticatedUserId;
                user.AuditDeleteDate = DateTime.Now;
                user.IsActive = false;

                var recordsAffected = await _unitOfWork.SaveChangesAsync();

                if (recordsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Data = true;
                    response.Message = ReplyMessage.MESSAGE_DELETE;
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
