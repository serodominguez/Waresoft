namespace Application.Interfaces
{
    public interface IGenerateExcelService
    {
        byte[] GenerateToExcel<T>(IEnumerable<T> data, List<(string ColumnName, string PropertyName)> columns, string? title = null, string? subtitle = null);
    }
}