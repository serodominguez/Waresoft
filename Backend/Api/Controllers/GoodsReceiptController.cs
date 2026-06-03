using Application.Commons.Bases.Request;
using Application.Commons.Settings;
using Application.Dtos.Request.GoodsReceipt;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Utilities.Extensions;
using Utilities.Static;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    public class GoodsReceiptController : BaseApiController
    {
        private readonly IGoodsReceiptService _goodsReceiptService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;
        private readonly string _frontendBaseUrl;

        public GoodsReceiptController(IGoodsReceiptService goodsReceiptService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService, IOptions<FrontendSettings> frontendSettings)
        {
            _goodsReceiptService = goodsReceiptService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
            _frontendBaseUrl = frontendSettings.Value.BaseUrl;
        }

        [HttpGet]
        [RequirePermission("Entrada de Productos", "Leer")]
        public async Task<IActionResult> ListGoodsReceipt([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _goodsReceiptService.ListGoodsReceiptByStore(AuthenticatedUserStoreId, filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsGoodsReceipt();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Entradas",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, "application/pdf", $"Entradas_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsGoodsReceipt();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response!.Data!, 
                        columnNames,
                        "Reporte de Entradas",
                        subtitle: $"{AuthenticatedUserStoreType} {AuthenticatedUserStoreName?.ToTitleCase() ?? ""}"
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel, $"Entradas_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }

            return Ok(response);
        }

        [HttpGet("{receiptId:int}")]
        [RequirePermission("Entrada de Productos", "Leer")]
        public async Task<IActionResult> GoodsReceiptById(int receiptId)
        {
            var response = await _goodsReceiptService.GoodsReceiptById(receiptId, AuthenticatedUserStoreId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Entrada de Productos", "Crear")]
        public async Task<IActionResult> RegisterGoodsReceipt([FromBody] GoodsReceiptRequestDto requestDto)
        {
            var response = await _goodsReceiptService.RegisterGoodsReceipt(AuthenticatedUserId, AuthenticatedUserStoreId, requestDto);
            return Ok(response);
        }

        [HttpPut("Disable/{receiptId:int}")]
        [RequirePermission("Entrada de Productos", "Eliminar")]
        public async Task<IActionResult> CancelGoodsReceipt(int receiptId)
        {
            var response = await _goodsReceiptService.CancelGoodsReceipt(AuthenticatedUserId, receiptId);
            return Ok(response);
        }

        [HttpGet("ExportPdf/{receiptId:int}")]
        [RequirePermission("Entrada de Productos", "Descargar")]
        //[Produces("application/pdf")]
        //[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportPdfGoodsReceipt(int receiptId)
        {
            var encodedId = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(receiptId.ToString()));
            var response = await _goodsReceiptService.GoodsReceiptById(receiptId, AuthenticatedUserStoreId);
            var qrUrl = $"{_frontendBaseUrl}/entradas/{encodedId}";
            var fileBytes = _generatePdfService.GoodsReceiptGeneratePdf(response.Data!,                
                AuthenticatedUserStoreType.ToTitleCase() ?? "",
                AuthenticatedUserStoreName.ToTitleCase() ?? "",
                qrUrl);

            var fileName = $"Entrada_{response.Data!.Code}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
