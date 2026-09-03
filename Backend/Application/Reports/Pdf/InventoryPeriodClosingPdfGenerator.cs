using Application.Dtos.Response.InventoryPeriod;
using Infrastructure.FilePdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Reports.Pdf
{
    public class InventoryPeriodClosingPdfGenerator : BasePdfGenerator
    {
        private readonly InventoryPeriodClosingResponseDto _closing;
        private readonly string _storeType;
        private readonly string _storeName;

        public InventoryPeriodClosingPdfGenerator(InventoryPeriodClosingResponseDto closing, string? storeType = null, string? storeName = null)
        {
            _closing = closing;
            _storeType = storeType ?? string.Empty;
            _storeName = storeName ?? string.Empty;
        }

        public override byte[] GeneratePdf()
        {
            var document = Document.Create(container =>
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

            return document.GeneratePdf().ToArray();
        }

        private void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(13).Bold().FontColor(Colors.Black);
            var subtitleStyle = TextStyle.Default.FontSize(11).SemiBold().FontColor(Colors.Black);

            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(titleCol =>
                    {
                        titleCol.Item().AlignCenter().Text("Cierre de Período").Style(titleStyle);
                        titleCol.Item().AlignCenter().Text($"{_storeType} {_storeName}").Style(subtitleStyle);
                        titleCol.Item().PaddingTop(5).AlignCenter()
                            .Text($"Generado el: {DateTime.Now:dd/MM/yyyy, h:mm:ss tt}")
                            .FontSize(9);
                    });
                });

                column.Item().PaddingTop(15);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Spacing(5);
                        left.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Período: ").SemiBold();
                            text.Span(_closing.PeriodName);
                        });
                        left.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Fecha Inicio: ").SemiBold();
                            text.Span(_closing.StartDate);
                        });
                        left.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Fecha de Apertura: ").SemiBold();
                            text.Span(_closing.OpenedDate);
                        });
                    });

                    row.RelativeItem().Column(right =>
                    {
                        right.Spacing(5);
                        right.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Estado: ").SemiBold();
                            text.Span(_closing.StatusPeriod);
                        });
                        right.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Fecha Fin: ").SemiBold();
                            text.Span(_closing.EndDate);
                        });
                        right.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Fecha de Cierre: ").SemiBold();
                            text.Span(_closing.ClosedDate ?? "—");
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
                    text.Span("Total Productos: ").SemiBold();
                    text.Span(_closing.TotalProducts.ToString());
                });

                column.Item().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(10));
                    text.Span("Stock Sistema: ").SemiBold();
                    text.Span(_closing.TotalSystemStock.ToString());
                });

                column.Item().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(10));
                    text.Span("Stock Físico: ").SemiBold();
                    text.Span(_closing.TotalPhysicalStock.ToString());
                });

                column.Item().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(10));
                    text.Span("Diferencia Total: ").SemiBold();
                    text.Span(_closing.TotalDifference.ToString()).Bold();
                });
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(1); 
                    cols.RelativeColumn(3);   
                    cols.RelativeColumn(5);
                    cols.RelativeColumn(2);    
                    cols.RelativeColumn(2);  
                    cols.RelativeColumn(2); 
                    cols.RelativeColumn(2); 
                    cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("Nº").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignLeft().Text("Código").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignLeft().Text("Descripción").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("Unidad").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Stk. Sistema").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Stk. Físico").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Diferencia").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Stk. Cierre").FontSize(10);

                    header.Cell().ColumnSpan(8).BorderBottom(1).BorderColor(Colors.Black).Height(0);
                });

                int i = 1;
                foreach (var item in _closing.Items)
                {
                    table.Cell().Element(BodyCellStyle).AlignCenter().Text(i++.ToString()).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignLeft().Text(item.ProductCode ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignLeft().Text(item.ProductDescription ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignCenter().Text(item.UnitMeasure ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(item.SystemStock.ToString()).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text((item.PhysicalStock?.ToString()) ?? "—").FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text((item.Difference?.ToString()) ?? "—").FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(item.ClosingStock.ToString()).FontSize(9);
                }
            });
        }
    }
}
