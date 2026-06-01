using Infrastructure.FilePdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using System.Runtime.InteropServices;
using ZXing;
using ZXing.Common;

namespace Application.Reports.Pdf
{
    public class ProductBarcodePdfGenerator : BasePdfGenerator
    {
        private const int ColumnsPerRow = 4;
        private const float LabelWidthCm = 3.8f;
        private const float LabelHeightCm = 1.8f;

        private readonly string _code;
        private readonly int _quantity;

        public ProductBarcodePdfGenerator(string code, int quantity)
        {
            _code = code;
            _quantity = quantity;
        }

        public override byte[] GeneratePdf()
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(1.0f, Unit.Centimetre);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeContent(IContainer container)
        {
            container.Grid(grid =>
            {
                grid.Columns(ColumnsPerRow);
                grid.Spacing(4);

                for (int i = 0; i < _quantity; i++)
                    grid.Item().Element(ComposeLabel);
            });
        }

        private void ComposeLabel(IContainer container)
        {
            var barcodeImage = GenerateBarcodeImage(_code);

            container
                .Border(0.5f)
                .BorderColor(Colors.Grey.Lighten2)
                .Width(LabelWidthCm, Unit.Centimetre)
                .Height(LabelHeightCm, Unit.Centimetre)
                .Padding(2)
                .Column(col =>
                {
                    col.Item().AlignCenter()
                       .Image(barcodeImage)
                       .FitWidth();

                    col.Item().AlignCenter()
                       .Text(_code)
                       .FontSize(6);
                });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.DefaultTextStyle(x => x.FontSize(8));
                text.Span("Página ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();
            });
        }

        private static byte[] GenerateBarcodeImage(string code)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 220,
                    Height = 60,
                    Margin = 2,
                    PureBarcode = true
                }
            };

            var pixelData = writer.Write(code);

            using var bitmap = new SKBitmap(pixelData.Width, pixelData.Height,
                                            SKColorType.Bgra8888, SKAlphaType.Opaque);
            Marshal.Copy(pixelData.Pixels, 0, bitmap.GetPixels(), pixelData.Pixels.Length);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
    }
}
