using Application.Commons.Bases.Request;
using Application.Dtos.Request.User;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public UserController(IUserService userService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _userService = userService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Usuarios", "Leer")]
        public async Task<IActionResult> ListUsers([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _userService.ListUsers(filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsUsers();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Usuarios",
                        AuthenticatedUserStoreName?.ToTitleCase() ?? ""
                    );
                    return File(fileBytes, "application/pdf", $"Usuarios_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsUsers();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response.Data!, 
                        columnNames,
                        "Reporte de Usuarios",
                        AuthenticatedUserStoreName?.ToTitleCase() ?? ""
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("Select")]
        [RequirePermission("Usuarios", "Leer")]
        public async Task<IActionResult> ListSelectUsers()
        {
            var response = await _userService.ListSelectUsers();
            return Ok(response);
        }

        [HttpGet("{userId:int}")]
        [RequirePermission("Usuarios", "Leer")]
        public async Task<IActionResult> UserById(int userId)
        {
            var response = await _userService.UserById(userId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Usuarios", "Crear")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRequestDto requestDto)
        {
            var response = await _userService.RegisterUser(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{userId:int}")]
        [RequirePermission("Usuarios", "Editar")]
        public async Task<IActionResult> EditUser(int userId, [FromBody] UserRequestDto requestDto)
        {
            var response = await _userService.EditUser(AuthenticatedUserId, userId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{userId:int}")]
        [RequirePermission("Usuarios", "Editar")]
        public async Task<IActionResult> EnableUser(int userId)
        {
            var response = await _userService.EnableUser(AuthenticatedUserId, userId);
            return Ok(response);
        }

        [HttpPut("Disable/{userId:int}")]
        [RequirePermission("Usuarios", "Editar")]
        public async Task<IActionResult> DisableUser(int userId)
        {
            var response = await _userService.DisableUser(AuthenticatedUserId, userId);
            return Ok(response);
        }

        [HttpPut("Remove/{userId:int}")]
        [RequirePermission("Usuarios", "Eliminar")]
        public async Task<IActionResult> RemoveUser(int userId)
        {
            var response = await _userService.RemoveUser(AuthenticatedUserId, userId);
            return Ok(response);
        }
    }
}
