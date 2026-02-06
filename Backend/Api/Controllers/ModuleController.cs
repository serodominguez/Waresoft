using Application.Commons.Bases.Request;
using Application.Dtos.Request.Module;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ModuleController : BaseApiController
    {
        private readonly IModuleService _moduleService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public ModuleController(IModuleService moduleService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _moduleService = moduleService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Módulos", "Leer")]
        public async Task<IActionResult> ListModules([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _moduleService.ListModules(filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsModules();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Módulos",
                        "Centro Optico" + " " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, "application/pdf", $"Módulos_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsModules();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response.Data!, 
                        columnNames,
                        "Reporte de Módulos",
                        "Centro Optico" + " " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("{moduleId:int}")]
        [RequirePermission("Módulos", "Leer")]
        public async Task<IActionResult> ModuleById(int moduleId)
        {
            var response = await _moduleService.ModuleById(moduleId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Módulos", "Crear")]
        public async Task<IActionResult> RegisterModule([FromBody] ModuleRequestDto requestDto)
        {
            var response = await _moduleService.RegisterModule(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{moduleId:int}")]
        [RequirePermission("Módulos", "Editar")]
        public async Task<IActionResult> EditModule(int moduleId, [FromBody] ModuleRequestDto requestDto)
        {
            var response = await _moduleService.EditModule(AuthenticatedUserId, moduleId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{moduleId:int}")]
        [RequirePermission("Módulos", "Editar")]
        public async Task<IActionResult> EnableModule(int moduleId)
        {
            var response = await _moduleService.EnableModule(AuthenticatedUserId, moduleId);
            return Ok(response);
        }

        [HttpPut("Disable/{moduleId:int}")]
        [RequirePermission("Módulos", "Editar")]
        public async Task<IActionResult> DisableModule(int moduleId)
        {
            var response = await _moduleService.DisableModule(AuthenticatedUserId, moduleId);
            return Ok(response);
        }

        [HttpPut("Remove/{moduleId:int}")]
        [RequirePermission("Módulos", "Eliminar")]
        public async Task<IActionResult> RemoveModule(int moduleId)
        {
            var response = await _moduleService.RemoveModule(AuthenticatedUserId, moduleId);
            return Ok(response);
        }
    }
}
