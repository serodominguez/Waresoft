using Application.Dtos.Response.GoodsReceipt;
using Infrastructure.FilePdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Reports.Pdf
{
    public class GoodsReceiptPdfGenerator : BasePdfGenerator
    {
        private readonly GoodsReceiptWithDetailsResponseDto _receipt;
        private readonly string _storeType;
        private readonly string _storeName;

        public GoodsReceiptPdfGenerator(GoodsReceiptWithDetailsResponseDto receipt, string? storeType = null, string? storeName = null)
        {
            _receipt = receipt;
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

            var pdfBytes = document.GeneratePdf();
            return pdfBytes.ToArray();
        }

        private void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(13).Bold().FontColor(Colors.Black);
            var subtitleStyle = TextStyle.Default.FontSize(11).SemiBold().FontColor(Colors.Black);

            container.Column(column =>
            {
                column.Item().AlignCenter().Text(text =>
                {
                    text.Span("Entrada de Productos ").Style(titleStyle);
                });

                column.Item().AlignCenter().Text($"{_storeType} {_storeName}").Style(subtitleStyle);
                
                column.Item().PaddingTop(5).AlignCenter()
                    .Text($"Generado el: {DateTime.Now:dd/MM/yyyy, h:mm:ss tt}")
                    .FontSize(9);

                column.Item().PaddingTop(15);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(leftColumn =>
                    {
                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Fecha de registro: ").SemiBold();
                            text.Span($"{_receipt.AuditCreateDate}");
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Código: ").SemiBold();
                            text.Span($"{_receipt.Code}");
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Tipo: ").SemiBold();
                            text.Span($"{_receipt.Type}");
                        });

                        leftColumn.Spacing(5);

                        if (_receipt.Type == "Adquisición")
                        {
                            leftColumn.Item().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Tipo de documento: ").SemiBold();
                                text.Span($"{_receipt.DocumentType}");
                            });

                            leftColumn.Spacing(5);
                        }
                    });

                    row.RelativeItem().Column(rightColumn =>
                    {
                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Registrado por: ").SemiBold();
                            text.Span($"{_receipt.AuditCreateName}");
                        });

                        rightColumn.Spacing(5);

                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Estado: ").SemiBold();
                            text.Span($"{_receipt.StatusReceipt}");
                        });

                        rightColumn.Spacing(5);

                        if (_receipt.Type == "Adquisición")
                        {
                            rightColumn.Item().AlignRight().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Proveedor: ").SemiBold();
                                text.Span($"{_receipt.CompanyName}");
                            });
                        }

                        rightColumn.Spacing(5);

                        if (_receipt.Type == "Adquisición")
                        {
                            rightColumn.Item().AlignRight().Text(text =>
                            {
                                text.DefaultTextStyle(x => x.FontSize(10));
                                text.Span("Número de documento: ").SemiBold();
                                text.Span($"{_receipt.DocumentNumber}");
                            });

                            rightColumn.Spacing(5);
                        }
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

                column.Item().AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(10));
                    text.Span("Total Bs.: ").SemiBold();
                    text.Span($"{FormatCurrency(_receipt.TotalAmount)}").Bold();
                });

                column.Item().PaddingTop(10).Element(container =>
                    ComposeObservations(container, _receipt.Annotations ?? string.Empty));
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(colums =>
                {
                    colums.RelativeColumn(1);
                    colums.RelativeColumn(2);
                    colums.RelativeColumn(4);
                    colums.RelativeColumn(3);
                    colums.RelativeColumn(3);
                    colums.RelativeColumn(2);
                    colums.RelativeColumn((float)1.5);
                    colums.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Nº").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Código").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Descripción").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Material").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Color").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Cantidad").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Costo").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Subtotal").FontSize(10);

                    header.Cell()
                        .ColumnSpan(8)
                        .BorderBottom(1)
                        .BorderColor(Colors.Black)
                        .Height(0);
                });

                foreach (var item in _receipt.GoodsReceiptDetails)
                {
                    table.Cell().Element(BodyCellStyle).Text(item.Item.ToString()).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(item.Code ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(item.Description ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(item.Material ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(item.Color ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(item.Quantity.ToString()).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(FormatCurrency(item.UnitCost)).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(FormatCurrency(item.TotalCost)).FontSize(9);
                }
            });
        }
    }
}