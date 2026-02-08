using Utilities.Static;

namespace Infrastructure.FilePdf
{
    public interface IListPdfGeneratorFactory
    {
        IBasePdfGenerator CreateGenerator<T>(
            IEnumerable<T> data,
            List<PdfTableColumn> columns,
            string title,
            string subtitle = "",
            bool showDate = true) where T : class;
    }
}
