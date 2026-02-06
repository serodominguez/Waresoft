using Application.Dtos.Response.GoodsIssue;
using Application.Dtos.Response.GoodsReceipt;
using Application.Dtos.Response.StoreInventory;
using Application.Generators.Pdf;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Utilities.Static;

namespace Application.Services
{
    public class GeneratePdfService : IGeneratePdfService
    {
        private readonly IConfiguration _configuration;

        public GeneratePdfService(IConfiguration configuration)
        {
            _configuration = configuration;
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

        public byte[] InventoryGeneratePdf(List<StoreInventoryResponseDto> inventory, string storeName)
        {
            var generator = new InventoryPdfGenerator(inventory, storeName);
            return generator.GeneratePdf();
        }

        public byte[] GenerateListPdf<T>(IEnumerable<T> data, List<(string ColumnName, string PropertyName)> columns, string title, string subtitle = "") where T : class
        {
            var pdfColumns = PdfColumnNames.GetColumns(columns);
            var generator = new ListPdfGenerator<T>(data, pdfColumns, title, subtitle);
            return generator.GeneratePdf();
        }
    }
}