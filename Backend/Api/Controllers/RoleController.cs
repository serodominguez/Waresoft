using Application.Commons.Bases.Request;
using Application.Dtos.Request.Role;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class RoleController : BaseApiController
    {
        private readonly IRoleService _roleService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public RoleController(IRoleService roleService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _roleService = roleService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Roles", "Leer")]
        public async Task<IActionResult> ListRoles([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _roleService.ListRoles(filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsRoles();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Roles",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, "application/pdf", $"Roles_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsRoles();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response.Data!, 
                        columnNames,
                        "Reporte de Roles",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);

                }
            }

            return Ok(response);
        }

        [HttpGet("Select")]
        [RequirePermission("Roles", "Leer")]
        public async Task<IActionResult> ListSelectRoles()
        {
            var response = await _roleService.ListSelectRoles();
            return Ok(response);
        }

        [HttpGet("{roleId:int}")]
        [RequirePermission("Roles", "Leer")]
        public async Task<IActionResult> RoleById(int roleId)
        {
            var response = await _roleService.RoleById(roleId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Roles", "Crear")]
        public async Task<IActionResult> RegisterRole([FromBody] RoleRequestDto requestDto)
        {
            var response = await _roleService.RegisterRole(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{roleId:int}")]
        [RequirePermission("Roles", "Editar")]
        public async Task<IActionResult> EditRole(int roleId, [FromBody] RoleRequestDto requestDto)
        {
            var response = await _roleService.EditRole(AuthenticatedUserId, roleId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{roleId:int}")]
        [RequirePermission("Roles", "Editar")]
        public async Task<IActionResult> EnableRole(int roleId)
        {
            var response = await _roleService.EnableRole(AuthenticatedUserId, roleId);
            return Ok(response);
        }

        [HttpPut("Disable/{roleId:int}")]
        [RequirePermission("Roles", "Editar")]
        public async Task<IActionResult> DisableRole(int roleId)
        {
            var response = await _roleService.DisableRole(AuthenticatedUserId, roleId);
            return Ok(response);
        }

        [HttpPut("Remove/{roleId:int}")]
        [RequirePermission("Roles", "Eliminar")]
        public async Task<IActionResult> RemoveRole(int roleId)
        {
            var response = await _roleService.RemoveRole(AuthenticatedUserId, roleId);
            return Ok(response);
        }
    }
}
