using Application.Interfaces;
using Infrastructure.FileExcel;
using Utilities.Static;

namespace Application.Services
{
    public class GenerateExcelService : IGenerateExcelService
    {
        private readonly IGenerateExcel _generateExcel;

        public GenerateExcelService(IGenerateExcel generateExcel)
        {
            _generateExcel = generateExcel;
        }

        public byte[] GenerateToExcel<T>(IEnumerable<T> data, List<(string ColumnName, string PropertyName)> columns, string? title = null, string? subtitle = null)
        {
            var excelColumns = ExcelColumnNames.GetColumns(columns);
            var memoryStreamExcel = _generateExcel.GenerateToExcel(data, excelColumns, title, subtitle);
            var fileBytes = memoryStreamExcel.ToArray();

            return fileBytes;
        }
    }
}