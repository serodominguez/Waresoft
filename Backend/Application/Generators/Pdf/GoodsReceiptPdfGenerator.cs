using Application.Dtos.Response.GoodsReceipt;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Generators.Pdf
{
    public class GoodsReceiptPdfGenerator : BasePdfGenerator
    {
        private readonly GoodsReceiptWithDetailsResponseDto _receipt;

        public GoodsReceiptPdfGenerator(GoodsReceiptWithDetailsResponseDto receipt)
        {
            _receipt = receipt;
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
            var titleStyle = TextStyle.Default.FontSize(12).Bold().FontColor(Colors.Black);

            container.Column(column =>
            {
                column.Item().AlignCenter().Text(text =>
                {
                    text.Span("Sucursal: ").Style(titleStyle);
                    text.Span(_receipt.StoreName).Style(titleStyle);
                });

                column.Item().AlignCenter().Text(text =>
                {
                    text.Span("Documento: ").Style(titleStyle);
                    text.Span(_receipt.Code).Style(titleStyle);
                });

                column.Item().PaddingTop(15);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(leftColumn =>
                    {
                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Tipo: ").SemiBold();
                            text.Span($"{_receipt.Type}");
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Fecha de Registro: ").SemiBold();
                            text.Span($"{_receipt.AuditCreateDate:d}");
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Tipo de Documento: ").SemiBold();
                            text.Span($"{_receipt.DocumentType}");
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Fecha del Documento: ").SemiBold();
                            text.Span($"{_receipt.DocumentDate:d}");
                        });
                    });

                    row.RelativeItem().Column(rightColumn =>
                    {
                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Proveedor: ").SemiBold();
                            text.Span($"{_receipt.CompanyName}");
                        });

                        rightColumn.Spacing(5);

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
                            text.Span("Número de Documento: ").SemiBold();
                            text.Span($"{_receipt.DocumentNumber}");
                        });
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Item().Element(ComposeTable);

                column.Spacing(10);

                column.Item().AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(10));
                    text.Span("Total Bs.: ").SemiBold();
                    text.Span($"{FormatCurrency(_receipt.TotalAmount)}").Bold();
                });

                column.Item().PaddingTop(25).Element(container =>
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
                    colums.RelativeColumn((float)1.5);
                    colums.RelativeColumn((float)1.5);
                    colums.RelativeColumn((float)1.5);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Nº").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Código").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Descripción").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Material").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).Text("Color").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Cantidad").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Costo U.").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Subtotal").FontSize(10);
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