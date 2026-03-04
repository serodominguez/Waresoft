using Utilities.Static;

namespace Infrastructure.FileExcel
{
    public interface IGenerateExcel
    {
        MemoryStream GenerateToExcel<T>(IEnumerable<T> data, List<ExcelTableColumn> columns, string? title = null, string? subtitle = null);
        MemoryStream GeneratePivotToExcel(IEnumerable<Dictionary<string, object?>> data, List<ExcelTableColumn> columns, string? title = null, string? subtitle = null);
        MemoryStream GenerateKardexToExcel<T>(IEnumerable<T> movements, List<ExcelTableColumn> columns, List<(string Label, string Value)> productInfo, string? title = null, string? subtitle = null);
    }
}