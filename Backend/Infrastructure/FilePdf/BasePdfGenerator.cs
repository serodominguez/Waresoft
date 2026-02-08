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
                .PaddingVertical(5)
                .BorderBottom(1)
                .BorderColor(Colors.Black);
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
                    leftColumn.Item().PaddingTop(40);
                    leftColumn.Item().BorderTop(1).BorderColor(Colors.Black).Text("").FontSize(9);
                    leftColumn.Item().AlignCenter().Text("Firma").FontSize(8);
                });

                row.ConstantItem(50);

                row.RelativeItem().Column(rightColumn =>
                {
                    rightColumn.Item().Text("Recibido por:").FontSize(10).SemiBold();
                    rightColumn.Item().PaddingTop(40);
                    rightColumn.Item().BorderTop(1).BorderColor(Colors.Black).Text("").FontSize(9);
                    rightColumn.Item().AlignCenter().Text("Firma").FontSize(8);
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

                column.Item().PaddingTop(30);
                column.Item().Element(ComposeSignatureSection);
            });
        }

        public abstract byte[] GeneratePdf();
    }
}