using Application.Commons.Bases.Request;
using Application.Dtos.Request.Product;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public ProductController(IProductService productService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _productService = productService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Productos", "Leer")]
        public async Task<IActionResult> ListProducts([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _productService.ListProducts(filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsProducts();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Productos",
                        AuthenticatedUserStoreName?.ToTitleCase() ?? ""
                    );
                    return File(fileBytes, "application/pdf", $"Productos_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsProducts();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response.Data!, 
                        columnNames,
                        "Reporte de Productos",
                        AuthenticatedUserStoreName?.ToTitleCase() ?? ""
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("{productId:int}")]
        [RequirePermission("Productos", "Leer")]
        public async Task<IActionResult> ProductById(int productId)
        {
            var response = await _productService.ProductById(productId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Productos", "Crear")]
        public async Task<IActionResult> RegisterProduct([FromBody] ProductRequestDto requestDto)
        {
            var response = await _productService.RegisterProduct(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Edit/{productId:int}")]
        [RequirePermission("Productos", "Editar")]
        public async Task<IActionResult> EditProduct(int productId, [FromBody] ProductRequestDto requestDto)
        {
            var response = await _productService.EditProduct(AuthenticatedUserId, productId, requestDto);
            return Ok(response);
        }

        [HttpPut("Enable/{productId:int}")]
        [RequirePermission("Productos", "Editar")]
        public async Task<IActionResult> EnableProduct(int productId)
        {
            var response = await _productService.EnableProduct(AuthenticatedUserId, productId);
            return Ok(response);
        }

        [HttpPut("Disable/{productId:int}")]
        [RequirePermission("Productos", "Editar")]
        public async Task<IActionResult> DisableProduct(int productId)
        {
            var response = await _productService.DisableProduct(AuthenticatedUserId, productId);
            return Ok(response);
        }

        [HttpPut("Remove/{productId:int}")]
        [RequirePermission("Productos", "Eliminar")]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            var response = await _productService.RemoveProduct(AuthenticatedUserId, productId);
            return Ok(response);
        }
    }
}
