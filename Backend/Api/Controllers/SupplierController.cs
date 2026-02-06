using Application.Commons.Bases.Request;
using Application.Dtos.Request.Supplier;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class SupplierController : BaseApiController
    {
        private readonly ISupplierService _supplierService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public SupplierController(ISupplierService supplierService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _supplierService = supplierService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Proveedores", "Leer")]
        public async Task<IActionResult> ListSuppliers([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _supplierService.ListSuppliers(filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsSuppliers();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Proveedores",
                        "Centro Optico" + " " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, "application/pdf", $"Proveedores_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsSuppliers();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response.Data!, 
                        columnNames,
                        "Reporte de Proveedores",
                        "Centro Optico" + " " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("Select")]
        [RequirePermission("Proveedores", "Leer")]
        public async Task<IActionResult> ListSelectSuppliers()
        {
            var response = await _supplierService.ListSelectSuppliers();
            return Ok(response);
        }

        [HttpGet("{supplierId:int}")]
        [RequirePermission("Proveedores", "Leer")]
        public async Task<IActionResult> SupplierById(int supplierId)
        {
            var response = await _supplierService.SupplierById(supplierId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Proveedores", "Crear")]
        public async Task<IActionResult> RegisterSupplier([FromBody] SupplierRequestDto requestDto)
        {
            var response = await _supplierService.RegisterSupplier(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{supplierId:int}")]
        [RequirePermission("Proveedores", "Editar")]
        public async Task<IActionResult> EditSupplier(int supplierId, [FromBody] SupplierRequestDto requestDto)
        {
            var response = await _supplierService.EditSupplier(AuthenticatedUserId, supplierId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{supplierId:int}")]
        [RequirePermission("Proveedores", "Editar")]
        public async Task<IActionResult> EnableSupplier(int supplierId)
        {
            var response = await _supplierService.EnableSupplier(AuthenticatedUserId, supplierId);
            return Ok(response);
        }

        [HttpPut("Disable/{supplierId:int}")]
        [RequirePermission("Proveedores", "Editar")]
        public async Task<IActionResult> DisableSupplier(int supplierId)
        {
            var response = await _supplierService.DisableSupplier(AuthenticatedUserId, supplierId);
            return Ok(response);
        }

        [HttpPut("Remove/{supplierId:int}")]
        [RequirePermission("Proveedores", "Eliminar")]
        public async Task<IActionResult> RemoveSupplier(int supplierId)
        {
            var response = await _supplierService.RemoveSupplier(AuthenticatedUserId, supplierId);
            return Ok(response);
        }
    }
}
