using Application.Commons.Bases.Request;
using Application.Dtos.Request.Transfer;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Static;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class TransferController : BaseApiController
    {
        private readonly ITransferService _transferService;
        private readonly IGenerateExcelService _generateExcelService;
        private readonly IGeneratePdfService _generatePdfService;

        public TransferController(ITransferService transferService, IGenerateExcelService generateExcelService, IGeneratePdfService generatePdfService)
        {
            _transferService = transferService;
            _generateExcelService = generateExcelService;
            _generatePdfService = generatePdfService;
        }


        [HttpGet]
        [RequirePermission("Traspaso de Productos", "Leer")]
        public async Task<IActionResult> ListTransfer([FromQuery] BaseFiltersRequest filters, [FromQuery] string? downloadType = "excel")
        {
            var response = await _transferService.ListTransferByStore(AuthenticatedUserStoreId, filters);

            if ((bool)filters.Download!)
            {
                if (downloadType?.ToLower() == "pdf")
                {
                    var columnNames = PdfColumnNames.GetColumnsTransfer();
                    var fileBytes = _generatePdfService.GenerateListPdf(
                        response.Data!,
                        columnNames,
                        "Reporte de Traspasos",
                        AuthenticatedUserStoreName?.ToTitleCase() ?? ""
                    );
                    return File(fileBytes, "application/pdf", $"Traspasos_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var columnNames = ExcelColumnNames.GetColumnsTransfer();
                    var fileBytes = _generateExcelService.GenerateToExcel(
                        response!.Data!,
                        columnNames,
                        "Reporte de Traspasos",
                        AuthenticatedUserStoreName?.ToTitleCase() ?? ""
                    );
                    return File(fileBytes, ContentType.ContentTypeExcel);
                }
            }

            return Ok(response);
        }

        [HttpGet("{transferId:int}")]
        [RequirePermission("Traspaso de Productos", "Leer")]
        public async Task<IActionResult> TransferById(int transferId)
        {
            var response = await _transferService.TransferById(AuthenticatedUserStoreId, transferId);
            return Ok(response);
        }

        [HttpPost("Send")]
        [RequirePermission("Traspaso de Productos", "Crear")]
        public async Task<IActionResult> SendTransfer([FromBody] TransferRequestDto requestDto)
        {
            var response = await _transferService.SendTransfer(AuthenticatedUserId, requestDto);
            return Ok(response);
        }

        [HttpPut("Receive/{transferId:int}")]
        [RequirePermission("Traspaso de Productos", "Editar")]
        public async Task<IActionResult> ReceiveTransfer(int transferId)
        {
            var response = await _transferService.ReceiveTransfer(AuthenticatedUserId, transferId);
            return Ok(response);
        }

        [HttpPut("Disable/{transferId:int}")]
        [RequirePermission("Traspaso de Productos", "Eliminar")]
        public async Task<IActionResult> CancelGoodsIssue(int transferId)
        {
            var response = await _transferService.CancelTransfer(AuthenticatedUserId, transferId);
            return Ok(response);
        }

        [HttpGet("ExportPdf/{transferId:int}")]
        [RequirePermission("Traspaso de Productos", "Descargar")]
        [Produces("application/pdf")]
        public async Task<IActionResult> ExportPdfTransfer(int transferId)
        {
            var response = await _transferService.TransferById(AuthenticatedUserId, transferId);
            var fileBytes = _generatePdfService.TransferGeneratePdf(response.Data!);

            var fileName = $"Traspaso_{response.Data!.Code}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
