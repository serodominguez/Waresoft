using Utilities.Static;

namespace Infrastructure.FileExcel
{
    public interface IGenerateExcel
    {
        MemoryStream GenerateToExcel<T>(IEnumerable<T> data, List<ExcelTableColumn> columns, string? title = null, string? subtitle = null);
        MemoryStream GenerateToExcel(IEnumerable<Dictionary<string, object?>> data, List<ExcelTableColumn> columns, string? title = null, string? subtitle = null);
    }
}