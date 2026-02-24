using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Role;
using Application.Dtos.Response.Role;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RoleRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public RoleService(IUnitOfWork unitOfWork, IValidator<RoleRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<RoleResponseDto>>> ListRoles(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<RoleResponseDto>>();

            try
            {
                var roles = _unitOfWork.Role.GetAllQueryable()
                    .AsNoTracking();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            roles = roles.Where(x => x.RoleName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    roles = roles.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                    roles = roles.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }
                response.TotalRecords = await roles.CountAsync();

                filters.Sort ??= "Id";
                var items = await _orderingQuery.Ordering(filters, roles, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(RoleMapp.RolesResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<RoleSelectResponseDto>>> ListSelectRoles()
        {
            var response = new BaseResponse<IEnumerable<RoleSelectResponseDto>>();

            try
            {
                var roles = (await _unitOfWork.Role.GetSelectAsync());

                if (roles is not null && roles.Any())
                {
                    response.Data = roles.Select(RoleMapp.RolesSelectResponseDtoMapping);
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
        public async Task<BaseResponse<RoleResponseDto>> RoleById(int roleId)
        {
            var response = new BaseResponse<RoleResponseDto>();

            try
            {
                var role = await _unitOfWork.Role.GetByIdAsync(roleId);

                if (role is not null)
                {
                    response.Data = RoleMapp.RolesResponseDtoMapping(role);
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

        public async Task<BaseResponse<bool>> RegisterRole(int authenticatedUserId, RoleRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            using var transaction = _unitOfWork.BeginTransaction();
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

                var entity = RoleMapp.RolesMapping(requestDto);
                entity.AuditCreateUser = authenticatedUserId;
                entity.AuditCreateDate = DateTime.Now;
                entity.Status = true;

                await _unitOfWork.Role.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var actions = (await _unitOfWork.Action.GetAllAsync())
                                    .Where(x => x.Status == true).ToList();

                var modules = (await _unitOfWork.Module.GetAllAsync())
                                    .Where(x => x.Status == true && x.Id != 1).ToList();

                var permissions = new List<PermissionEntity>();
                foreach (var module in modules)
                {
                    foreach (var action in actions)
                    {
                        permissions.Add(new PermissionEntity
                        {
                            IdRole = entity.Id,
                            IdModule = module.Id,
                            IdAction = action.Id,
                            AuditCreateUser = authenticatedUserId,
                            AuditCreateDate = DateTime.Now,
                            Status = false
                        });
                    }
                }

                await _unitOfWork.Permission.AddRangeAsync(permissions);
                await _unitOfWork.SaveChangesAsync();

                transaction.Commit();
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> EditRole(int authenticatedUserId, int roleId, RoleRequestDto requestDto)
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

                var isValid = await _unitOfWork.Role.GetByIdAsync(roleId);
                if (isValid is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var role = RoleMapp.RolesMapping(requestDto);
                role.Id = roleId;
                role.AuditUpdateUser = authenticatedUserId;
                role.AuditUpdateDate = DateTime.Now;

                response.Data = await _unitOfWork.Role.EditAsync(role);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
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

        public async Task<BaseResponse<bool>> EnableRole(int authenticatedUserId, int roleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var role = await _unitOfWork.Role.GetByIdAsync(roleId);

                if (role is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                role.AuditUpdateUser = authenticatedUserId;
                role.AuditUpdateDate = DateTime.Now;
                role.Status = true;

                response.Data = await _unitOfWork.Role.UpdateAsync(role);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
                }
                else
                {
                    response.IsSuccess = false;
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

        public async Task<BaseResponse<bool>> DisableRole(int authenticatedUserId, int roleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var role = await _unitOfWork.Role.GetByIdAsync(roleId);

                if (role is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                role.AuditUpdateUser = authenticatedUserId;
                role.AuditUpdateDate = DateTime.Now;
                role.Status = false;

                response.Data = await _unitOfWork.Role.UpdateAsync(role);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_INACTIVATE;
                }
                else
                {
                    response.IsSuccess = false;
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

        public async Task<BaseResponse<bool>> RemoveRole(int authenticatedUserId, int roleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var role = await _unitOfWork.Role.GetByIdAsync(roleId);

                if (role is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                role.AuditDeleteUser = authenticatedUserId;
                role.AuditDeleteDate = DateTime.Now;
                role.Status = false;

                response.Data = await _unitOfWork.Role.RemoveAsync(role);

                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_DELETE;
                }
                else
                {
                    response.IsSuccess = false;
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
