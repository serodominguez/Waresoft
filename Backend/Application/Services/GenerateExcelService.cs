using Application.Interfaces;
using Application.Dtos.Response.StoreInventory;
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
            var columns = new List<(string ColumnName, string PropertyName)>
            {
                ("Código", "Code"),
                ("Color", "Color"),
                ("Marca", "BrandName"),
                ("Categoría", "CategoryName"),
                ("Fecha Creación", "AuditCreateDate"),
            };

            foreach (var store in data.Stores)
                columns.Add((store, store));

            var flatRows = data.Rows.Select(row =>
            {
                var dict = new Dictionary<string, object?>
                {
                    ["Code"] = row.Code,
                    ["Color"] = row.Color,
                    ["BrandName"] = row.BrandName,
                    ["CategoryName"] = row.CategoryName,
                    ["AuditCreateDate"] = row.AuditCreateDate,
                };

                foreach (var store in data.Stores)
                    dict[store] = row.StockByStore!.TryGetValue(store, out var val) ? val : 0;

                return dict;
            }).ToList();

            var excelColumns = ExcelColumnNames.GetColumns(columns);
            var memoryStream = _generateExcel.GenerateToExcel(flatRows, excelColumns, title, subtitle);
            return memoryStream.ToArray();
        }
    }
}