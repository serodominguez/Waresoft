using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;
using Utilities.Static;

namespace Infrastructure.FilePdf
{
    public class ListPdfGenerator<T> : BasePdfGenerator where T : class
    {
        private readonly IEnumerable<T> _data;
        private readonly List<PdfTableColumn> _columns;
        private readonly string _title;
        private readonly string _subtitle;
        private readonly bool _showDate;

        public ListPdfGenerator(
            IEnumerable<T> data,
            List<PdfTableColumn> columns,
            string title,
            string subtitle = "",
            bool showDate = true)
        {
            _data = data;
            _columns = columns;
            _title = title;
            _subtitle = subtitle;
            _showDate = showDate;
        }

        public override byte[] GeneratePdf()
        {
            Document document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));
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
            var titleStyle = TextStyle.Default.FontSize(14).Bold().FontColor(Colors.Black);
            var subtitleStyle = TextStyle.Default.FontSize(11).SemiBold().FontColor(Colors.Black);

            container.Column(column =>
            {
                column.Item().AlignCenter().Text(_title).Style(titleStyle);

                if (!string.IsNullOrWhiteSpace(_subtitle))
                {
                    column.Item().AlignCenter().Text(_subtitle).Style(subtitleStyle);
                }

                if (_showDate)
                {
                    column.Item().AlignCenter()
                        .Text($"Fecha: {DateTime.Now:dd/MM/yyyy, h:mm:ss tt}")
                        .FontSize(9);
                }

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
                // Definir columnas con anchos relativos
                table.ColumnsDefinition(columns =>
                {
                    foreach (var column in _columns)
                    {
                        columns.RelativeColumn(column.RelativeWidth);
                    }
                });

                // Encabezado
                table.Header(header =>
                {
                    foreach (var column in _columns)
                    {
                        var cell = header.Cell().Element(HeaderCellStyle);

                        cell = ApplyAlignment(cell, column.Alignment);
                        cell.Text(column.Label ?? "").FontSize(9);
                    }
                });

                // Filas de datos
                foreach (var item in _data)
                {
                    foreach (var column in _columns)
                    {
                        var cell = table.Cell().Element(BodyCellStyle);
                        cell = ApplyAlignment(cell, column.Alignment);

                        var value = GetPropertyValue(item, column.PropertyName ?? "");
                        var displayValue = FormatValue(value, column);

                        cell.Text(displayValue).FontSize(8);
                    }
                }
            });
        }

        private IContainer ApplyAlignment(IContainer container, PdfColumnAlignment alignment)
        {
            return alignment switch
            {
                PdfColumnAlignment.Center => container.AlignCenter(),
                PdfColumnAlignment.Right => container.AlignRight(),
                _ => container.AlignLeft()
            };
        }

        protected override IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .DefaultTextStyle(x => x.SemiBold())
                .PaddingVertical(5)
                .PaddingHorizontal(3)
                .BorderBottom(1)
                .BorderColor(Colors.Black);
        }

        protected override IContainer BodyCellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(3)
                .PaddingHorizontal(3);
        }

        private object? GetPropertyValue(T item, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return null;

            // Soporte para propiedades anidadas (ej: "User.Name")
            var properties = propertyName.Split('.');
            object? value = item;

            foreach (var prop in properties)
            {
                if (value == null)
                    return null;

                var propertyInfo = value.GetType().GetProperty(prop,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (propertyInfo == null)
                    return null;

                value = propertyInfo.GetValue(value);
            }

            return value;
        }

        private string FormatValue(object? value, PdfTableColumn column)
        {
            if (value == null)
                return "";

            // Formatear según el tipo de dato
            return value switch
            {
                DateTime dateTime => dateTime.ToString("dd/MM/yyyy"),
                decimal decimalValue => FormatCurrency(decimalValue),
                double doubleValue => FormatCurrency((decimal)doubleValue),
                float floatValue => FormatCurrency((decimal)floatValue),
                bool boolValue => boolValue ? "Sí" : "No",
                _ => value.ToString() ?? ""
            };
        }
    }
}