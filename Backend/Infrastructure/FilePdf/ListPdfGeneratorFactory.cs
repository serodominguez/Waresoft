using Utilities.Static;

namespace Infrastructure.FilePdf
{
    public class ListPdfGeneratorFactory : IListPdfGeneratorFactory
    {
        public IBasePdfGenerator CreateGenerator<T>(
            IEnumerable<T> data,
            List<PdfTableColumn> columns,
            string title,
            string subtitle = "",
            bool showDate = true) where T : class
        {
            return new ListPdfGenerator<T>(data, columns, title, subtitle, showDate);
        }
    }
}
