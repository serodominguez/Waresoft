using Application.Dtos.Response.Transfer;
using Infrastructure.FilePdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Reports
{
    public class TransferPdfGenerator : BasePdfGenerator
    {
        private readonly TransferWithDetailsResponseDto _transfer;

        public TransferPdfGenerator(TransferWithDetailsResponseDto transfer)
        {
            _transfer = transfer;
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
                    text.Span("Traspaso de Productos").Style(titleStyle);
                });

                column.Item().AlignCenter().Text(text =>
                {
                    text.Span("Sucursal: ").Style(titleStyle);
                    text.Span(_transfer.StoreOrigin).Style(titleStyle);

                });

                column.Item().PaddingTop(15);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(leftColumn =>
                    {
                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Código: ").SemiBold();
                            text.Span($"{_transfer.Code}");
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Fecha de envio: ").SemiBold();
                            text.Span($"{_transfer.SendDate}");
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Origen: ").SemiBold();
                            text.Span($"{_transfer.StoreOrigin}");
                        });

                        leftColumn.Spacing(5);

                        leftColumn.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Enviado por: ").SemiBold();
                            text.Span($"{_transfer.AuditCreateName}");
                        });
                    });

                    row.RelativeItem().Column(rightColumn =>
                    {
                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Estado: ").SemiBold();
                            text.Span($"{_transfer.StatusTransfer}");
                        });

                        rightColumn.Spacing(5);

                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Fecha de recepción: ").SemiBold();
                            text.Span($"{_transfer.ReceiveDate}");
                        });

                        rightColumn.Spacing(5);

                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Destino: ").SemiBold();
                            text.Span($"{_transfer.AuditUpdateName}");
                        });

                        rightColumn.Spacing(5);

                        rightColumn.Item().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));
                            text.Span("Recibido por: ").SemiBold();
                            text.Span($"{_transfer.AuditUpdateName}");
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
                    text.Span($"{FormatCurrency(_transfer.TotalAmount)}").Bold();
                });

                column.Item().PaddingTop(25).Element(container =>
                    ComposeObservations(container, _transfer.Annotations ?? string.Empty));
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
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Precio U.").FontSize(10);
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Subtotal").FontSize(10);
                });

                foreach (var item in _transfer.TransferDetails)
                {
                    table.Cell().Element(BodyCellStyle).Text(item.Item.ToString()).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(item.Code ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(item.Description ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(item.Material ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).Text(item.Color ?? string.Empty).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(item.Quantity.ToString()).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(FormatCurrency(item.UnitPrice)).FontSize(9);
                    table.Cell().Element(BodyCellStyle).AlignRight().Text(FormatCurrency(item.TotalPrice)).FontSize(9);
                }
            });
        }
    }
}
