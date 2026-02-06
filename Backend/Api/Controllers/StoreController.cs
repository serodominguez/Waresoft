using Application.Commons.Bases.Request;
using Application.Dtos.Request.Store;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class StoreController : BaseApiController
    {
        private readonly IStoreService _storeService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public StoreController(IStoreService storeService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _storeService = storeService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Establecimientos", "Leer")]
        public async Task<IActionResult> ListStores([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _storeService.ListStores(filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsStores();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Establecimientos",
                        "Centro Optico" + " " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, "application/pdf", $"Establecimientos_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsStores();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response.Data!, 
                        columnNames,
                        "Reporte de Establecimientos",
                        "Centro Optico" + " " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("Select")]
        [RequirePermission("Establecimientos", "Leer")]
        public async Task<IActionResult> ListSelectStores()
        {
            var response = await _storeService.ListSelectStores();
            return Ok(response);
        }

        [HttpGet("{storeId:int}")]
        [RequirePermission("Establecimientos", "Leer")]
        public async Task<IActionResult> StoreById(int storeId)
        {
            var response = await _storeService.StoreById(storeId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Establecimientos", "Crear")]
        public async Task<IActionResult> RegisterStore([FromBody] StoreRequestDto requestDto)
        {
            var response = await _storeService.RegisterStore(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{storeId:int}")]
        [RequirePermission("Establecimientos", "Editar")]
        public async Task<IActionResult> EditStore(int storeId, [FromBody] StoreRequestDto requestDto)
        {
            var response = await _storeService.EditStore(AuthenticatedUserId, storeId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{storeId:int}")]
        [RequirePermission("Establecimientos", "Editar")]
        public async Task<IActionResult> EnableStore(int storeId)
        {
            var response = await _storeService.EnableStore(AuthenticatedUserId, storeId);
            return Ok(response);
        }

        [HttpPut("Disable/{storeId:int}")]
        [RequirePermission("Establecimientos", "Editar")]
        public async Task<IActionResult> DisableStore(int storeId)
        {
            var response = await _storeService.DisableStore(AuthenticatedUserId, storeId);
            return Ok(response);
        }

        [HttpPut("Remove/{storeId:int}")]
        [RequirePermission("Establecimientos", "Eliminar")]
        public async Task<IActionResult> RemoveStore(int storeId)
        {
            var response = await _storeService.RemoveStore(AuthenticatedUserId, storeId);
            return Ok(response);
        }
    }
}
