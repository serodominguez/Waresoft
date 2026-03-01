using Application.Commons.Bases.Request;
using Application.Dtos.Request.Brand;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class BrandController : BaseApiController
    {
        private readonly IBrandService _brandService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public BrandController(IBrandService brandService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _brandService = brandService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Marcas", "Leer")]
        public async Task<IActionResult> ListBrands([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _brandService.ListBrands(filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsBrands();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Marcas",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, "application/pdf", $"Marcas_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsBrands();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response.Data!,
                        columnNames,
                        "Reporte de Marcas",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("Select")]
        [RequirePermission("Marcas", "Leer")]
        public async Task<IActionResult> ListSelectBrands()
        {
            var response = await _brandService.ListSelectBrands();
            return Ok(response);
        }

        [HttpGet("{brandId:int}")]
        [RequirePermission("Marcas", "Leer")]
        public async Task<IActionResult> BrandById(int brandId)
        {
            var response = await _brandService.BrandById(brandId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Marcas", "Crear")]
        public async Task<IActionResult> RegisterBrand([FromBody] BrandRequestDto requestDto)
        {
            var response = await _brandService.RegisterBrand(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{brandId:int}")]
        [RequirePermission("Marcas", "Editar")]
        public async Task<IActionResult> EditBrand(int brandId, [FromBody] BrandRequestDto requestDto)
        {
            var response = await _brandService.EditBrand(AuthenticatedUserId, brandId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{brandId:int}")]
        [RequirePermission("Marcas", "Editar")]
        public async Task<IActionResult> EnableBrand(int brandId)
        {
            var response = await _brandService.EnableBrand(AuthenticatedUserId, brandId);
            return Ok(response);
        }

        [HttpPut("Disable/{brandId:int}")]
        [RequirePermission("Marcas", "Editar")]
        public async Task<IActionResult> DisableBrand(int brandId)
        {
            var response = await _brandService.DisableBrand(AuthenticatedUserId, brandId);
            return Ok(response);
        }

        [HttpPut("Remove/{brandId:int}")]
        [RequirePermission("Marcas", "Eliminar")]
        public async Task<IActionResult> RemoveBrand(int brandId)
        {
            var response = await _brandService.RemoveBrand(AuthenticatedUserId, brandId);
            return Ok(response);
        }
    }
}
