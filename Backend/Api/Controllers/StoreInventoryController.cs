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
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
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
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel, $"Inventario_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }

            return Ok(response);
        }

        [HttpGet("Pivot")]
        [RequirePermission("Inventario", "Leer")]
        public async Task<IActionResult> ListInventoryPivot([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _storeInventoryService.ListInventoryPivot(filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var fileBytes = _generatePdfService.PivotInventoryGeneratePdf(
                        response.Data!,
                        AuthenticatedUserStoreType,
                        AuthenticatedUserStoreName?.ToTitleCase() ?? string.Empty
                    );
                    return File(fileBytes, "application/pdf", $"InventarioPivot_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var fileBytes = _generateExcelService.GeneratePivotInventoryToExcel(
                        response.Data!,
                        "Reporte de existencias",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel, $"InventarioPivot_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }

            return Ok(response);
        }

        [HttpGet("Kardex")]
        [RequirePermission("Inventario", "Leer")]
        public async Task<IActionResult> ListInventoryKardex([FromQuery] int productId, [FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _storeInventoryService.ListKardexInventory(AuthenticatedUserStoreId, productId, filters);
            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var fileBytes = _generatePdfService.KardexGeneratePdf(
                        response.Data!,
                        AuthenticatedUserStoreType,
                        AuthenticatedUserStoreName?.ToTitleCase() ?? string.Empty
                    );
                    return File(fileBytes, "application/pdf", $"Kardex_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var fileBytes = _generateExcelService.GenerateKardexToExcel(
                        response.Data!,
                        "Reporte de Kardex",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel, $"Kardex_{DateTime.Now:yyyyMMdd}.xlsx");
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
                var fileBytes = _generatePdfService.InventoryGeneratePdf(
                    response.Data!.ToList(),
                    AuthenticatedUserStoreType,
                    AuthenticatedUserStoreName.ToTitleCase() ?? string.Empty
                );
                return File(fileBytes, "application/pdf", $"Inventario_{DateTime.Now:yyyyMMdd}.pdf");
            }

            return Ok(response);
        }
    }
}
