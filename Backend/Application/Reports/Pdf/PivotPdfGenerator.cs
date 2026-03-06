using Application.Dtos.Response.StoreInventory;
using Infrastructure.FilePdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Reports.Pdf
{
    public class PivotPdfGenerator : BasePdfGenerator
    {
        private readonly StoreInventoryPivotResponseDto _pivot;
        private readonly string _storeType;
        private readonly string _storeName;

        public PivotPdfGenerator(StoreInventoryPivotResponseDto pivot, string? storeType = null, string? storeName = null)
        {
            _pivot = pivot;
            _storeType = storeType ?? string.Empty;
            _storeName = storeName ?? string.Empty;
        }

        public override byte[] GeneratePdf()
        {
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Landscape para acomodar múltiples columnas de tiendas
                    page.Size(PageSizes.Letter.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeStandardFooter);
                });
            });

            return document.GeneratePdf();
        }
        private void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(13).Bold().FontColor(Colors.Black);
            var subtitleStyle = TextStyle.Default.FontSize(11).SemiBold().FontColor(Colors.Black);

            container.Column(column =>
            {
                column.Item().AlignCenter().Text("Reporte Consolidado de Existencias").Style(titleStyle);
                column.Item().AlignCenter().Text($"{_storeType} {_storeName}").Style(subtitleStyle);

                column.Item().PaddingTop(5).AlignCenter()
                    .Text($"Generado el: {DateTime.Now:dd/MM/yyyy, h:mm:ss tt}")
                    .FontSize(9);

                column.Item().PaddingTop(6);
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
            var stores = _pivot.Stores;

            container.Table(table =>
            {
                // Columnas fijas: Código, Color, Marca, Categoría, Fecha
                // + una columna por cada tienda
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);   // Código
                    columns.RelativeColumn(2);   // Color
                    columns.RelativeColumn(2.5f); // Marca
                    columns.RelativeColumn(3);   // Categoría
                    columns.RelativeColumn(2.5f); // Fecha

                    foreach (var _ in stores)
                        columns.RelativeColumn(1.8f); // Stock por tienda
                });

                // Encabezados
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Código").FontSize(9);
                    header.Cell().Element(HeaderCellStyle).Text("Color").FontSize(9);
                    header.Cell().Element(HeaderCellStyle).Text("Marca").FontSize(9);
                    header.Cell().Element(HeaderCellStyle).Text("Categoría").FontSize(9);
                    header.Cell().Element(HeaderCellStyle).Text("Fecha Alta").FontSize(9);

                    foreach (var store in stores)
                        header.Cell().Element(HeaderCellStyle).AlignCenter().Text(store).FontSize(9);

                    // Línea separadora bajo encabezado
                    header.Cell()
                        .ColumnSpan((uint)(5 + stores.Count))
                        .BorderBottom(1)
                        .BorderColor(Colors.Black)
                        .Height(0);
                });

                // Filas de datos
                foreach (var row in _pivot.Rows)
                {
                    table.Cell().Element(BodyCellStyle).Text(row.Code ?? string.Empty).FontSize(8);
                    table.Cell().Element(BodyCellStyle).Text(row.Color ?? string.Empty).FontSize(8);
                    table.Cell().Element(BodyCellStyle).Text(row.BrandName ?? string.Empty).FontSize(8);
                    table.Cell().Element(BodyCellStyle).Text(row.CategoryName ?? string.Empty).FontSize(8);
                    table.Cell().Element(BodyCellStyle).Text(row.AuditCreateDate ?? string.Empty).FontSize(8);

                    foreach (var store in stores)
                    {
                        row.StockByStore!.TryGetValue(store, out var stock);
                        table.Cell().Element(BodyCellStyle).AlignCenter()
                            .Text(stock.ToString()).FontSize(8);
                    }
                }
            });
        }
    }
}
