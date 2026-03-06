using Application.Dtos.Response.StoreInventory;
using Infrastructure.FilePdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Reports.Pdf
{
    public class KardexPdfGenerator : BasePdfGenerator
    {
        private readonly StoreInventoryKardexResponseDto _kardex;
        private readonly string _storeType;
        private readonly string _storeName;

        public KardexPdfGenerator(StoreInventoryKardexResponseDto kardex, string? storeType = null, string? storeName = null)
        {
            _kardex = kardex;
            _storeType = storeType ?? string.Empty;
            _storeName = storeName ?? string.Empty;
        }

        public override byte[] GeneratePdf()
        {
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeStandardFooter);
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(12).Bold().FontColor(Colors.Black);

            container.Column(column =>
            {
                column.Item().AlignCenter().Text("Kardex de Inventario").Style(titleStyle);
                column.Item().AlignCenter().Text($"{_storeType} {_storeName}").Style(titleStyle);
                column.Item().PaddingTop(15);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(leftColumn =>
                    {
                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Código: ").SemiBold();
                            text.Span(_kardex.Code ?? string.Empty);
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Descripción: ").SemiBold();
                            text.Span(_kardex.Description ?? string.Empty);
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Material: ").SemiBold();
                            text.Span(_kardex.Material ?? string.Empty);
                        });
                    });

                    row.RelativeItem().Column(rightColumn =>
                    {
                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Color: ").SemiBold();
                            text.Span(_kardex.Color ?? string.Empty);
                        });

                        rightColumn.Spacing(5);

                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Unidad de Medida: ").SemiBold();
                            text.Span(_kardex.UnitMeasure ?? string.Empty);
                        });

                        rightColumn.Spacing(5);

                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Stock Actual: ").SemiBold();
                            text.Span(_kardex.CurrentStock.ToString());
                        });
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(15).Column(column =>
            {
                column.Item().Element(ComposeTable);

                column.Spacing(10);

                column.Item().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(10));
                    text.Span("Stock Calculado: ").SemiBold();
                    text.Span(_kardex.CalculatedStock.ToString()).Bold();
                });

                column.Item().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(10));
                    text.Span("Diferencia: ").SemiBold();
                    var diff = _kardex.StockDifference;
                    text.Span(diff.ToString()).Bold();
                });
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Código").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Fecha").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Movimiento").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Tipo").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Estado").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Cantidad").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Acumulado").FontSize(10);

                    header.Cell()
                        .ColumnSpan(7)
                        .BorderBottom(1)
                        .BorderColor(Colors.Black)
                        .Height(0);
                });

                foreach (var movement in _kardex.Movements)
                {
                    var quantityColor = movement.Quantity < 0 ? Colors.Red.Medium : Colors.Green.Medium;

                    table.Cell().Element(BodyCellStyle).Text(movement.Code ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(movement.Date ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(movement.MovementType ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(movement.Type ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(movement.State ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight()
                        .Text(movement.Quantity.ToString()).FontSize(9).FontColor(quantityColor);
                    table.Cell().Element(BodyCellStyle).AlignRight()
                        .Text(movement.AccumulatedStock.ToString()).FontSize(9);
                }
            });
        }
    }
}
