using Application.Commons.Bases.Response;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Utilities.Static;

namespace Api.Filters
{
    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IPermissionService _permissionApplication;

        public PermissionAuthorizationFilter(IPermissionService permissionApplication)
        {
            _permissionApplication = permissionApplication;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var permissionAttribute = endpoint?.Metadata.GetMetadata<RequirePermissionAttribute>();

            if (permissionAttribute == null)
                return;

            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Result = new JsonResult(new BaseResponse<object>
                {
                    IsSuccess = false,
                    Message = ReplyMessage.MESSAGE_UNAUTHORIZED
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };

                return;
            }

            var hasPermission = await _permissionApplication.UserPermissions(userId, permissionAttribute.Module, permissionAttribute.Action);

            if (!hasPermission)
            {
                context.Result = new JsonResult(new BaseResponse<object>
                {
                    IsSuccess = false,
                    Message = ReplyMessage.MESSAGE_FORBIDDEN
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
