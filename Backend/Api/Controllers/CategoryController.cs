using Application.Commons.Bases.Request;
using Application.Dtos.Request.Category;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : BaseApiController
    {
        private readonly ICategoryService _categoryService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public CategoryController(ICategoryService categoryService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _categoryService = categoryService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Categorías", "Leer")]
        public async Task<IActionResult> ListCategories([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _categoryService.ListCategories(filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsCategories();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Categorías",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, "application/pdf", $"Categorías_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsCategories();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response.Data!, 
                        columnNames,
                        "Reporte de Categorías",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("Select")]
        [RequirePermission("Categorías", "Leer")]
        public async Task<IActionResult> ListSelectCategories()
        {
            var response = await _categoryService.ListSelectCategories();
            return Ok(response);
        }

        [HttpGet("{categoryId:int}")]
        [RequirePermission("Categorías", "Leer")]
        public async Task<IActionResult> CategoryById(int categoryId)
        {
            var response = await _categoryService.CategoryById(categoryId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Categorías", "Crear")]
        public async Task<IActionResult> RegisterCategory([FromBody] CategoryRequestDto requestDto)
        {
            var response = await _categoryService.RegisterCategory(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{categoryId:int}")]
        [RequirePermission("Categorías", "Editar")]
        public async Task<IActionResult> EditCategory(int categoryId, [FromBody] CategoryRequestDto requestDto)
        {
            var response = await _categoryService.EditCategory(AuthenticatedUserId, categoryId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{categoryId:int}")]
        [RequirePermission("Categorías", "Editar")]
        public async Task<IActionResult> EnableCategory(int categoryId)
        {
            var response = await _categoryService.EnableCategory(AuthenticatedUserId, categoryId);
            return Ok(response);
        }

        [HttpPut("Disable/{categoryId:int}")]
        [RequirePermission("Categorías", "Editar")]
        public async Task<IActionResult> DisableCategory(int categoryId)
        {
            var response = await _categoryService.DisableCategory(AuthenticatedUserId, categoryId);
            return Ok(response);
        }

        [HttpPut("Remove/{categoryId:int}")]
        [RequirePermission("Categorías", "Eliminar")]
        public async Task<IActionResult> RemoveCategory(int categoryId)
        {
            var response = await _categoryService.RemoveCategory(AuthenticatedUserId, categoryId);
            return Ok(response);
        }
    }
}
