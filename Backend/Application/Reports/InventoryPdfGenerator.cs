using Application.Dtos.Response.StoreInventory;
using Infrastructure.FilePdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Utilities.Extensions;

namespace Application.Reports
{
    public class InventoryPdfGenerator : BasePdfGenerator
    {
        private readonly List<StoreInventoryResponseDto> _inventory;
        private readonly string _storeName;

        public InventoryPdfGenerator(List<StoreInventoryResponseDto> inventory, string storeName)
        {
            _inventory = inventory;
            _storeName = storeName;
        }

        public override byte[] GeneratePdf()
        {
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeInventoryFooter);
                });
            });

            var pdfBytes = document.GeneratePdf();
            return pdfBytes.ToArray();
        }

        private void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(14).Bold().FontColor(Colors.Black);
            var subtitleStyle = TextStyle.Default.FontSize(11).SemiBold().FontColor(Colors.Black);

            container.Column(column =>
            {
                column.Item().AlignCenter().Text("Centro Optico" + ' ' + _storeName.ToTitleCase()).Style(titleStyle);
                column.Item().AlignCenter().Text("Planilla de inventario").Style(subtitleStyle);
                column.Item().AlignCenter().Text($"Fecha: {DateTime.Now:dd/MM/yyyy, h:mm:ss tt}").FontSize(9);
                column.Item().PaddingTop(10);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Item().Element(ComposeTable);
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);      // Categoría
                    columns.RelativeColumn(1.5f);   // Marca
                    columns.RelativeColumn(2);      // Descripción
                    columns.RelativeColumn(1.5f);   // Código
                    columns.RelativeColumn(1.5f);   // Color
                    columns.RelativeColumn(1.5f);   // Material
                    columns.RelativeColumn(1);      // Precio
                    columns.RelativeColumn(1);      // Cantidad
                });

                table.Header(header =>
                {
                    header.Cell().Element(InventoryHeaderCellStyle).Text("Categoría").FontSize(9);
                    header.Cell().Element(InventoryHeaderCellStyle).Text("Marca").FontSize(9);
                    header.Cell().Element(InventoryHeaderCellStyle).Text("Descripción").FontSize(9);
                    header.Cell().Element(InventoryHeaderCellStyle).Text("Código").FontSize(9);
                    header.Cell().Element(InventoryHeaderCellStyle).Text("Color").FontSize(9);
                    header.Cell().Element(InventoryHeaderCellStyle).Text("Material").FontSize(9);
                    header.Cell().Element(InventoryHeaderCellStyle).AlignRight().Text("Precio").FontSize(9);
                    header.Cell().Element(InventoryHeaderCellStyle).AlignRight().Text("Cantidad").FontSize(9);

                    header.Cell()
                        .ColumnSpan(8)
                        .BorderBottom(1)
                        .BorderColor(Colors.Black)
                        .Height(0);
                });

                foreach (var item in _inventory)
                {
                    table.Cell().Element(InventoryBodyCellStyle).Text(item.CategoryName ?? "").FontSize(8);
                    table.Cell().Element(InventoryBodyCellStyle).Text(item.BrandName ?? "").FontSize(8);
                    table.Cell().Element(InventoryBodyCellStyle).Text(item.Description ?? "").FontSize(8);
                    table.Cell().Element(InventoryBodyCellStyle).Text(item.Code ?? "").FontSize(8);
                    table.Cell().Element(InventoryBodyCellStyle).Text(item.Color ?? "").FontSize(8);
                    table.Cell().Element(InventoryBodyCellStyle).Text(item.Material ?? "").FontSize(8);
                    table.Cell().Element(InventoryBodyCellStyle).AlignRight().Text(FormatCurrency(item.Price)).FontSize(8);
                    table.Cell().Element(InventoryBodyCellStyle).AlignRight().Text($"{item.StockAvailable}/........").FontSize(8);
                }
            });
        }

        private IContainer InventoryHeaderCellStyle(IContainer container)
        {
            return container
                .DefaultTextStyle(x => x.SemiBold())
                .PaddingVertical(2)
                .PaddingHorizontal(3);
        }

        private IContainer InventoryBodyCellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(3)
                .PaddingHorizontal(3);
        }

        private void ComposeInventoryFooter(IContainer container)
        {
            container.AlignCenter()
                .Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(9));
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
        }
    }
}