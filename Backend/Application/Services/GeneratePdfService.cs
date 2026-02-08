using Application.Dtos.Response.GoodsIssue;
using Application.Dtos.Response.GoodsReceipt;
using Application.Dtos.Response.StoreInventory;
using Application.Dtos.Response.Transfer;
using Application.Interfaces;
using Application.Reports;
using Infrastructure.FilePdf;
using Microsoft.Extensions.Configuration;
using Utilities.Static;

namespace Application.Services
{
    public class GeneratePdfService : IGeneratePdfService
    {
        private readonly IConfiguration _configuration;
        private readonly IListPdfGeneratorFactory _listPdfFactory;

        public GeneratePdfService(IConfiguration configuration, IListPdfGeneratorFactory listPdfFactory)
        {
            _configuration = configuration;
            _listPdfFactory = listPdfFactory;
        }

        public byte[] GoodsIssueGeneratePdf(GoodsIssueWithDetailsResponseDto issue)
        {
            var generator = new GoodsIssuePdfGenerator(issue);
            return generator.GeneratePdf();
        }

        public byte[] GoodsReceiptGeneratePdf(GoodsReceiptWithDetailsResponseDto receipt)
        {
            var generator = new GoodsReceiptPdfGenerator(receipt);
            return generator.GeneratePdf();
        }

        public byte[] TransferGeneratePdf(TransferWithDetailsResponseDto transfer)
        {
            var generator = new TransferPdfGenerator(transfer);
            return generator.GeneratePdf();
        }

        public byte[] InventoryGeneratePdf(List<StoreInventoryResponseDto> inventory, string storeName)
        {
            var generator = new InventoryPdfGenerator(inventory, storeName);
            return generator.GeneratePdf();
        }

        public byte[] GenerateListPdf<T>(IEnumerable<T> data, List<(string ColumnName, string PropertyName)> columns, string title, string subtitle = "") where T : class
        {
            var pdfColumns = PdfColumnNames.GetColumns(columns);
            var generator = _listPdfFactory.CreateGenerator(data, pdfColumns, title, subtitle);
            return generator.GeneratePdf();
        }
    }
}