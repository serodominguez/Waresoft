using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace Infrastructure.FilePdf
{
    public abstract class BasePdfGenerator : IBasePdfGenerator
    {
        protected readonly CultureInfo _bolivianCulture;

        protected BasePdfGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            _bolivianCulture = new CultureInfo("es-BO");
            _bolivianCulture.NumberFormat.NumberGroupSeparator = ",";
            _bolivianCulture.NumberFormat.NumberDecimalDigits = 0;
        }

        protected string FormatCurrency(decimal value)
        {
            return value.ToString("N0", _bolivianCulture);
        }

        protected void ComposeStandardFooter(IContainer container)
        {
            container.AlignCenter()
                .Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(10));
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
        }

        protected virtual IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .DefaultTextStyle(x => x.SemiBold())
                .PaddingVertical(2)
                .PaddingHorizontal(3);
        }

        protected virtual IContainer BodyCellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(1);
        }

        protected void ComposeSignatureSection(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(leftColumn =>
                {
                    leftColumn.Item().Text("Entregado por:").FontSize(10).SemiBold();
                    leftColumn.Item().PaddingTop(30);
                    leftColumn.Item().Element(signLine =>
                    {
                        signLine.Column(col =>
                        {
                            col.Item().Height(0).BorderBottom(1).BorderColor(Colors.Black);
                        });
                    });
                    leftColumn.Item().PaddingTop(4).AlignCenter().Text("Firma").FontSize(8);
                });

                row.ConstantItem(50);

                row.RelativeItem().Column(rightColumn =>
                {
                    rightColumn.Item().Text("Recibido por:").FontSize(10).SemiBold();
                    rightColumn.Item().PaddingTop(30);
                    rightColumn.Item().Element(signLine =>
                    {
                        signLine.Column(col =>
                        {
                            col.Item().Height(0).BorderBottom(1).BorderColor(Colors.Black);
                        });
                    });
                    rightColumn.Item().PaddingTop(4).AlignCenter().Text("Firma").FontSize(8);
                });
            });
        }

        protected void ComposeSingleSignatureSection(IContainer container, string signatureLabel)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Column(centerColumn =>
                {
                    centerColumn.Item().Text(signatureLabel).FontSize(10).SemiBold();
                    centerColumn.Item().PaddingTop(30);
                    centerColumn.Item().Width(250).Element(signLine =>
                    {
                        signLine.Column(col =>
                        {
                            col.Item().Height(0).BorderBottom(1).BorderColor(Colors.Black);
                        });
                    });
                    centerColumn.Item().PaddingTop(4).AlignCenter().Text("Firma").FontSize(8);
                });
            });
        }

        protected void ComposeObservations(IContainer container, string annotations)
        {
            container.Column(column =>
            {
                column.Item()
                    .Background(Colors.Grey.Lighten3)
                    .Padding(10)
                    .Column(innerColumn =>
                    {
                        innerColumn.Spacing(5);
                        innerColumn.Item().Text("Observaciones:").FontSize(10);
                        innerColumn.Item().Text(annotations ?? "").FontSize(9);
                    });

                column.Item().PaddingTop(15);
                column.Item().Element(ComposeSignatureSection);
            });
        }
        protected void ComposeObservationsWithSingleSignature(IContainer container, string annotations, string signatureLabel)
        {
            container.Column(column =>
            {
                column.Item()
                    .Background(Colors.Grey.Lighten3)
                    .Padding(10)
                    .Column(innerColumn =>
                    {
                        innerColumn.Spacing(5);
                        innerColumn.Item().Text("Observaciones:").FontSize(10);
                        innerColumn.Item().Text(annotations ?? "").FontSize(9);
                    });

                column.Item().PaddingTop(15);
                column.Item().Element(c => ComposeSingleSignatureSection(c, signatureLabel));
            });
        }

        public abstract byte[] GeneratePdf();
    }
}