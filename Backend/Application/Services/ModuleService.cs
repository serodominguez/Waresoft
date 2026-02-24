using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Module;
using Application.Dtos.Response.Module;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ModuleRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public ModuleService(IUnitOfWork unitOfWork, IValidator<ModuleRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<ModuleResponseDto>>> ListModules(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<ModuleResponseDto>>();

            try
            {
                var modules = _unitOfWork.Module.GetAllQueryable()
                    .AsNoTracking();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            modules = modules.Where(x => x.ModuleName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                if (filters.StateFilter is not null)
                {
                    var stateValue = Convert.ToBoolean(filters.StateFilter);
                    modules = modules.Where(x => x.Status == stateValue);
                }

                if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
                {
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                    modules = modules.Where(x => x.AuditCreateDate >= startDate && x.AuditCreateDate < endDate);
                }
                response.TotalRecords = await modules.CountAsync();

                filters.Sort ??= "Id";
                var items = await _orderingQuery.Ordering(filters, modules, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(ModuleMapp.ModulesResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<ModuleResponseDto>> ModuleById(int moduleId)
        {

            var response = new BaseResponse<ModuleResponseDto>();

            try
            {
                var module = await _unitOfWork.Module.GetByIdAsync(moduleId);

                if (module is not null)
                {
                    response.Data = ModuleMapp.ModulesResponseDtoMapping(module);
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

        public async Task<BaseResponse<bool>> RegisterModule(int authenticatedUserId, ModuleRequestDto requestDto)
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

                var module = ModuleMapp.ModulesMapping(requestDto);
                module.AuditCreateUser = authenticatedUserId;
                module.AuditCreateDate = DateTime.Now;
                module.Status = true;

                await _unitOfWork.Module.AddAsync(module);
                await _unitOfWork.SaveChangesAsync(); 

                var actions = (await _unitOfWork.Action.GetAllAsync())
                                    .Where(x => x.Status == true).ToList();

                var roles = (await _unitOfWork.Role.GetAllAsync())
                                    .Where(x => x.Status == true).ToList();

                var permissions = new List<PermissionEntity>();
                foreach (var role in roles)
                {
                    foreach (var action in actions)
                    {
                        permissions.Add(new PermissionEntity
                        {
                            IdRole = role.Id,
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

        public async Task<BaseResponse<bool>> EditModule(int authenticatedUserId, int moduleId, ModuleRequestDto requestDto)
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

                var module = await _unitOfWork.Module.GetByIdForUpdateAsync(moduleId);

                if (module is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                module.ModuleName = requestDto.ModuleName; 
                module.AuditUpdateUser = authenticatedUserId;
                module.AuditUpdateDate = DateTime.Now;

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

        public async Task<BaseResponse<bool>> EnableModule(int authenticatedUserId, int moduleId)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var module = await _unitOfWork.Module.GetByIdForUpdateAsync(moduleId);

                if (module is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                module.Status = true;
                module.AuditUpdateUser = authenticatedUserId;
                module.AuditUpdateDate = DateTime.Now;

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

        public async Task<BaseResponse<bool>> DisableModule(int authenticatedUserId, int moduleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var module = await _unitOfWork.Module.GetByIdForUpdateAsync(moduleId);

                if (module is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                module.AuditUpdateUser = authenticatedUserId;
                module.AuditUpdateDate = DateTime.Now;
                module.Status = false;

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

        public async Task<BaseResponse<bool>> RemoveModule(int authenticatedUserId, int moduleId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var module = await _unitOfWork.Module.GetByIdForUpdateAsync(moduleId);

                if (module is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                module.AuditDeleteUser = authenticatedUserId;
                module.AuditDeleteDate = DateTime.Now;
                module.Status = false;

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
    }
}
