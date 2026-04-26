using Application.Dtos.Response.StoreInventory;
using Infrastructure.FileExcel;
using Utilities.Static;

namespace Application.Reports.Excel
{
    public class ConsolidatedExcelGenerator
    {
        private readonly StoreInventoryPivotResponseDto _data;
        private readonly string? _title;
        private readonly string? _subtitle;
        private readonly IGenerateExcel _generateExcel;

        public ConsolidatedExcelGenerator(StoreInventoryPivotResponseDto data, IGenerateExcel generateExcel, string? title = null, string? subtitle = null)
        {
            _data = data;
            _generateExcel = generateExcel;
            _title = title;
            _subtitle = subtitle;
        }

        public byte[] GenerateExcel()
        {
            var columns = new List<(string ColumnName, string PropertyName)>
            {
                ("Código", "Code"),
                ("Descripción", "Description"),
                ("Color", "Color"),
                ("Marca", "BrandName"),
                ("Categoría", "CategoryName"),
                ("Fecha Creación", "AuditCreateDate"),
            };

            foreach (var store in _data.Stores)
                columns.Add((store, store));

            var flatRows = _data.Rows.Select(row =>
            {
                var dict = new Dictionary<string, object?>
                {
                    ["Code"] = row.Code,
                    ["Description"] = row.Description,
                    ["Color"] = row.Color,
                    ["BrandName"] = row.BrandName,
                    ["CategoryName"] = row.CategoryName,
                    ["AuditCreateDate"] = row.AuditCreateDate,
                };

                foreach (var store in _data.Stores)
                    dict[store] = row.StockByStore!.TryGetValue(store, out var val) ? val : 0;

                return dict;
            }).ToList();

            var excelColumns = ExcelColumnNames.GetColumns(columns);
            var memoryStream = _generateExcel.GeneratePivotToExcel(flatRows, excelColumns, _title, _subtitle);

            return memoryStream.ToArray();
        }
    }
}
