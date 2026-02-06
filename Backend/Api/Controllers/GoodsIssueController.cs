using Application.Commons.Bases.Request;
using Application.Dtos.Request.GoodsIssue;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class GoodsIssueController : BaseApiController
    {
        private readonly IGoodsIssueService _goodsIssueService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public GoodsIssueController(IGoodsIssueService goodsIssueService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _goodsIssueService = goodsIssueService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }

        [HttpGet]
        [RequirePermission("Salida de Productos", "Leer")]
        public async Task<IActionResult> ListGoodsIssue([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _goodsIssueService.ListGoodsIssueByStore(AuthenticatedUserStoreId, filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsGoodsIssue();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Salidas",
                        "Centro Optico" + " " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, "application/pdf", $"Salidas_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsGoodsIssue();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response!.Data!, 
                        columnNames,
                        "Reporte de Salidas",
                        "Centro Optico" + " " + AuthenticatedUserStoreName.ToTitleCase()
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("{issueId:int}")]
        [RequirePermission("Salida de Productos", "Leer")]
        public async Task<IActionResult> GoodsIssueById(int issueId)
        {
            var response = await _goodsIssueService.GoodsIssueById(issueId);
            return Ok(response);
        }

        [HttpPost("Register")]
        [RequirePermission("Salida de Productos", "Crear")]
        public async Task<IActionResult> RegisterGoodsIssue([FromBody] GoodsIssueRequestDto requestDto)
        {
            var response = await _goodsIssueService.RegisterGoodsIssue(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Disable/{issueId:int}")]
        [RequirePermission("Salida de Productos", "Eliminar")]
        public async Task<IActionResult> CancelGoodsIssue(int issueId)
        {
            var response = await _goodsIssueService.CancelGoodsIssue(AuthenticatedUserId, issueId);
            return Ok(response);
        }

        [HttpGet("ExportPdf/{issueId:int}")]
        [RequirePermission("Salida de Productos", "Descargar")]
        [Produces("application/pdf")]
        public async Task<IActionResult> ExportPdfGoodsIssue(int issueId)
        {
            var response = await _goodsIssueService.GoodsIssueById(issueId);
            var fileBytes = _generatePdfService.GoodsIssueGeneratePdf(response.Data!);

            var fileName = $"Salida_{response.Data!.Code}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
