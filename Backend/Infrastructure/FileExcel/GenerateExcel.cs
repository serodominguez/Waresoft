using ClosedXML.Excel;
using Utilities.Static;

namespace Infrastructure.FileExcel
{
    public class GenerateExcel : IGenerateExcel
    {
        public MemoryStream GenerateToExcel<T>(IEnumerable<T> data, List<ExcelTableColumn> columns, string? title = null, string? subtitle = null)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Listado");

            int startRow = 1;

            // Agregar título si existe
            if (!string.IsNullOrEmpty(title))
            {
                var titleCell = worksheet.Cell(startRow, 1);
                titleCell.Value = title;
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Font.FontSize = 16;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(startRow, 1, startRow, columns.Count).Merge();
                startRow++;
            }

            // Agregar subtítulo si existe
            if (!string.IsNullOrEmpty(subtitle))
            {
                var subtitleCell = worksheet.Cell(startRow, 1);
                subtitleCell.Value = subtitle;
                subtitleCell.Style.Font.Bold = true;
                subtitleCell.Style.Font.FontSize = 12;
                subtitleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(startRow, 1, startRow, columns.Count).Merge();
                startRow++;
            }

            // Agregar fecha y hora
            var dateCell = worksheet.Cell(startRow, 1);
            dateCell.Value = $"Fecha: {DateTime.Now:dd/MM/yyyy, h:mm tt}";
            dateCell.Style.Font.FontSize = 10;
            dateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(startRow, 1, startRow, columns.Count).Merge();
            startRow++;

            // Espacio entre cabecera y columnas
            startRow++;

            // Agregar encabezados de columnas
            for (int i = 0; i < columns.Count; i++)
            {
                var cell = worksheet.Cell(startRow, i + 1);
                cell.Value = columns[i].Label;
                cell.Style.Font.Bold = true;
            }

            var rowIndex = startRow + 1;

            // Agregar datos
            foreach (var item in data)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    var propertyValue = typeof(T).GetProperty(columns[i].PropertyName!)?.GetValue(item)?.ToString();
                    worksheet.Cell(rowIndex, i + 1).Value = propertyValue;
                }

                rowIndex++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}