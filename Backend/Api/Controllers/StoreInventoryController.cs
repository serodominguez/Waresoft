using Application.Commons.Bases.Request;
using Application.Dtos.Request.StoreInventory;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class StoreInventoryController : BaseApiController
    {
        private readonly IStoreInventoryService _storeInventoryService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public StoreInventoryController(IStoreInventoryService storeInventoryService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _storeInventoryService = storeInventoryService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Inventario", "Leer")]
        public async Task<IActionResult> ListInventory([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _storeInventoryService.ListInventory(AuthenticatedUserStoreId, filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsInventories();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Inventario",
                        "Centro Optico" + " " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, "application/pdf", $"Inventario_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsInventories();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response.Data!,
                        columnNames,
                        "Reporte de Inventario",
                        "Centro Optico " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpPut("Edit")]
        [RequirePermission("Inventario", "Editar")]
        public async Task<IActionResult> EditInventory([FromBody] StoreInventoryRequestDto requestDto)
        {
            var response = await _storeInventoryService.UpdatePriceByProduct(AuthenticatedUserStoreId, AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpGet("ExportPdf")]
        [RequirePermission("Inventario", "Descargar")]
        public async Task<IActionResult> InventorySheet([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "pdf")
        {
            var response = await _storeInventoryService.ListInventory(AuthenticatedUserStoreId, filters);

            if ((bool)filters.Download!)
            {
                var fileBytes = _generatePdfService.InventoryGeneratePdf(response.Data!.ToList(), AuthenticatedUserStoreName);
                return File(fileBytes, "application/pdf", $"Inventario_{DateTime.Now:yyyyMMdd}.pdf");
            }

            return Ok(response);
        }
    }
}
