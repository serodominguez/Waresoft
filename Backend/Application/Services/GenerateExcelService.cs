using Application.Dtos.Response.StoreInventory;
using Application.Interfaces;
using Application.Reports.Excel;
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

        public byte[] GeneratePivotInventoryToExcel(StoreInventoryPivotResponseDto data, string? title = null, string? subtitle = null)
        {
            var generator = new ConsolidatedExcelGenerator(data, _generateExcel, title, subtitle);
            return generator.GenerateExcel();
        }

        public byte[] GenerateKardexToExcel(StoreInventoryKardexResponseDto data, string? title = null, string? subtitle = null)
        {
            var generator = new KardexExcelGenerator(data, _generateExcel, title, subtitle);
            return generator.GenerateExcel();
        }
    }
}