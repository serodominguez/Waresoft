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
                    var property = typeof(T).GetProperty(columns[i].PropertyName!);
                    if (property != null)
                    {
                        var value = property.GetValue(item);

                        if (value != null)
                        {
                            var cell = worksheet.Cell(rowIndex, i + 1);

                            // Asignar el valor según su tipo real
                            switch (value)
                            {
                                case int intValue:
                                    cell.Value = intValue;
                                    break;
                                case long longValue:
                                    cell.Value = longValue;
                                    break;
                                case decimal decimalValue:
                                    cell.Value = decimalValue;
                                    break;
                                case double doubleValue:
                                    cell.Value = doubleValue;
                                    break;
                                case float floatValue:
                                    cell.Value = floatValue;
                                    break;
                                case DateTime dateValue:
                                    cell.Value = dateValue;
                                    cell.Style.NumberFormat.Format = "dd/MM/yyyy";
                                    break;
                                case bool boolValue:
                                    cell.Value = boolValue;
                                    break;
                                default:
                                    cell.Value = value.ToString();
                                    break;
                            }

                            //// Aplicar formatos específicos según la columna
                            //if (columns[i].PropertyName == "Precio" ||
                            //    columns[i].PropertyName == "Monto" ||
                            //    columns[i].PropertyName == "Total")
                            //{
                            //    cell.Style.NumberFormat.Format = "$#,##0.00";
                            //}
                            //else if (columns[i].PropertyName == "Porcentaje")
                            //{
                            //    cell.Style.NumberFormat.Format = "0.00%";
                            //}
                            //else if (columns[i].PropertyName == "Cantidad")
                            //{
                            //    cell.Style.NumberFormat.Format = "#,##0";
                            //}
                        }
                    }
                }
                rowIndex++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        public MemoryStream GeneratePivotToExcel(IEnumerable<Dictionary<string, object?>> data, List<ExcelTableColumn> columns, string? title = null, string? subtitle = null)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Listado");

            int startRow = 1;

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

            var dateCell = worksheet.Cell(startRow, 1);
            dateCell.Value = $"Fecha: {DateTime.Now:dd/MM/yyyy, h:mm tt}";
            dateCell.Style.Font.FontSize = 10;
            dateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(startRow, 1, startRow, columns.Count).Merge();
            startRow++;

            startRow++;

            for (int i = 0; i < columns.Count; i++)
            {
                var cell = worksheet.Cell(startRow, i + 1);
                cell.Value = columns[i].Label;
                cell.Style.Font.Bold = true;
            }

            var rowIndex = startRow + 1;

            foreach (var item in data)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    if (item.TryGetValue(columns[i].PropertyName!, out var val) && val != null)
                    {
                        var cell = worksheet.Cell(rowIndex, i + 1);

                        switch (val)
                        {
                            case int intValue:
                                cell.Value = intValue;
                                break;
                            case long longValue:
                                cell.Value = longValue;
                                break;
                            case decimal decimalValue:
                                cell.Value = decimalValue;
                                break;
                            case double doubleValue:
                                cell.Value = doubleValue;
                                break;
                            case float floatValue:
                                cell.Value = floatValue;
                                break;
                            case DateTime dateValue:
                                cell.Value = dateValue;
                                break;
                            case bool boolValue:
                                cell.Value = boolValue;
                                break;
                            default:
                                cell.Value = val.ToString();
                                break;
                        }
                    }
                }
                rowIndex++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        public MemoryStream GenerateKardexToExcel<T>(
    IEnumerable<T> movements,
    List<ExcelTableColumn> columns,
    List<(string Label, string Value)> productInfo,
    string? title = null,
    string? subtitle = null)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Kardex");

            int startRow = 1;

            // Título
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

            // Subtítulo
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

            // Fecha
            var dateCell = worksheet.Cell(startRow, 1);
            dateCell.Value = $"Fecha: {DateTime.Now:dd/MM/yyyy, h:mm tt}";
            dateCell.Style.Font.FontSize = 10;
            dateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(startRow, 1, startRow, columns.Count).Merge();
            startRow++;

            startRow++; // espacio

            // Datos del producto (cabecera) — dos columnas: Label | Valor
            foreach (var (label, value) in productInfo)
            {
                var labelCell = worksheet.Cell(startRow, 1);
                labelCell.Value = label;
                labelCell.Style.Font.Bold = true;
                labelCell.Style.Font.FontSize = 10;

                var valueCell = worksheet.Cell(startRow, 2);
                valueCell.Value = value;
                valueCell.Style.Font.FontSize = 10;

                startRow++;
            }

            startRow++; // espacio entre cabecera y tabla

            // Encabezados de columnas de movimientos
            for (int i = 0; i < columns.Count; i++)
            {
                var cell = worksheet.Cell(startRow, i + 1);
                cell.Value = columns[i].Label;
                cell.Style.Font.Bold = true;
            }

            var rowIndex = startRow + 1;

            // Filas de movimientos
            foreach (var item in movements)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    var property = typeof(T).GetProperty(columns[i].PropertyName!);
                    if (property == null) continue;

                    var value = property.GetValue(item);
                    if (value == null) continue;

                    var cell = worksheet.Cell(rowIndex, i + 1);

                    switch (value)
                    {
                        case int intValue:
                            cell.Value = intValue;
                            break;
                        case long longValue:
                            cell.Value = longValue;
                            break;
                        case decimal decimalValue:
                            cell.Value = decimalValue;
                            break;
                        case double doubleValue:
                            cell.Value = doubleValue;
                            break;
                        case float floatValue:
                            cell.Value = floatValue;
                            break;
                        case DateTime dateValue:
                            cell.Value = dateValue;
                            cell.Style.NumberFormat.Format = "dd/MM/yyyy";
                            break;
                        case bool boolValue:
                            cell.Value = boolValue;
                            break;
                        default:
                            cell.Value = value.ToString();
                            break;
                    }
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