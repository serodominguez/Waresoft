using Application.Commons.Bases.Request;
using Application.Dtos.Request.GoodsReceipt;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class GoodsReceiptController : BaseApiController
    {
        private readonly IGoodsReceiptService _goodsReceiptService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public GoodsReceiptController(IGoodsReceiptService goodsReceiptService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _goodsReceiptService = goodsReceiptService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
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
                        AuthenticatedUserStoreName?.ToTitleCase() ?? ""
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
                        AuthenticatedUserStoreName?.ToTitleCase() ?? ""
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("{receiptId:int}")]
        [RequirePermission("Entrada de Productos", "Leer")]
        public async Task<IActionResult> GoodsReceiptById(int receiptId)
        {
            var response = await _goodsReceiptService.GoodsReceiptById(receiptId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Entrada de Productos", "Crear")]
        public async Task<IActionResult> RegisterGoodsReceipt([FromBody] GoodsReceiptRequestDto requestDto)
        {
            var response = await _goodsReceiptService.RegisterGoodsReceipt(AuthenticatedUserId, requestDto);
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
        [Produces("application/pdf")]
        //[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportPdfGoodsReceipt(int receiptId)
        {
            var response = await _goodsReceiptService.GoodsReceiptById(receiptId);
            var fileBytes = _generatePdfService.GoodsReceiptGeneratePdf(response.Data!);

            var fileName = $"Entrada_{response.Data!.Code}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }

        //[HttpGet("ExportPdf/{receiptId:int}")]
        //[RequirePermission("Ingreso de Productos", "Leer")]
        //public async Task<IActionResult> ExportPdfGoodsReceipt(int receiptId)
        //{
        //    var response = await _goodsReceiptService.ExportPdfGoodsReceipt(receiptId);
        //    var fileBytes = _generatePdfService.GoodsReceiptGeneratePdf(response.Data!);
        //    return File(fileBytes, ContentType.ContentTypePdf);
        //}
    }
}
