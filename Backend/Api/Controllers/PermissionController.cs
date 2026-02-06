using Application.Dtos.Request.Permission;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class PermissionController : BaseApiController
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("User")]
        //[AllowAnonymous]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UserPermissions()
        {
            var userId = AuthenticatedUserId;
            var response = await _permissionService.ListUserPermissions(userId);
            return Ok(response);
        }

        [HttpGet("Role/{roleId:int}")]
        [RequirePermission("Permisos", "Leer")]
        public async Task<IActionResult> RolePermissions(int roleId)
        {
            var response = await _permissionService.PermissionsByRole(roleId);
            return Ok(response);
        }

        [HttpPut("Update")]
        [RequirePermission("Permisos", "Editar")]
        public async Task<IActionResult> UpdatePermissions([FromBody] List<PermissionRequestDto> permissionDto)
        {

            var response = await _permissionService.UpdatePermissions(AuthenticatedUserId, permissionDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
