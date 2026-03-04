using Application.Dtos.Response.StoreInventory;
using Infrastructure.FileExcel;
using Utilities.Static;

namespace Application.Reports.Excel
{
    public class KardexExcelGenerator
    {
        private readonly StoreInventoryKardexResponseDto _data;
        private readonly IGenerateExcel _generateExcel;
        private readonly string? _title;
        private readonly string? _subtitle;

        public KardexExcelGenerator(StoreInventoryKardexResponseDto data, IGenerateExcel generateExcel, string? title = null, string? subtitle = null)
        {
            _data = data;
            _generateExcel = generateExcel;
            _title = title;
            _subtitle = subtitle;
        }

        public byte[] GenerateExcel()
        {
            var productInfo = new List<(string Label, string Value)>
            {
                ("Código",           _data.Code ?? string.Empty),
                ("Descripción",      _data.Description ?? string.Empty),
                ("Material",         _data.Material ?? string.Empty),
                ("Color",            _data.Color ?? string.Empty),
                ("Unidad de Medida", _data.UnitMeasure ?? string.Empty),
                ("Stock Actual",     _data.CurrentStock.ToString()),
                ("Stock Calculado",  _data.CalculatedStock.ToString()),
                ("Diferencia",       _data.StockDifference.ToString()),
            };

            var movementColumns = new List<(string ColumnName, string PropertyName)>
            {
                ("Código",      "Code"),
                ("Fecha",       "Date"),
                ("Movimiento",  "MovementType"),
                ("Tipo",        "Type"),
                ("Cantidad",    "Quantity"),
                ("Acumulado",   "AccumulatedStock"),
                ("Estado",      "State"),
            };

            var excelColumns = ExcelColumnNames.GetColumns(movementColumns);
            var memoryStream = _generateExcel.GenerateKardexToExcel(
                _data.Movements,
                excelColumns,
                productInfo,
                _title,
                _subtitle
            );

            return memoryStream.ToArray();
        }
    }
}
