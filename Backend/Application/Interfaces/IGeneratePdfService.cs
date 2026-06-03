using Application.Dtos.Response.GoodsIssue;
using Application.Dtos.Response.GoodsReceipt;
using Application.Dtos.Response.Product;
using Application.Dtos.Response.StoreInventory;
using Application.Dtos.Response.Transfer;

namespace Application.Interfaces
{
    public interface IGeneratePdfService
    {
        byte[] GoodsIssueGeneratePdf(GoodsIssueWithDetailsResponseDto issue, string storeType, string storeName, string qrUrl);
        byte[] GoodsReceiptGeneratePdf(GoodsReceiptWithDetailsResponseDto receipt, string storeType, string storeName, string qrUrl);
        byte[] InventoryGeneratePdf(List<StoreInventoryCalculatedResponseDto> inventory, string storeType, string storeName);
        byte[] KardexGeneratePdf(StoreInventoryKardexResponseDto kardex, string storeType, string storeName);
        byte[] PivotInventoryGeneratePdf(StoreInventoryPivotResponseDto pivot, string storeType, string storeName);
        byte[] ProductBarcodeGeneratePdf(ProductSelectResponseDto product, int quantity);
        byte[] TransferGeneratePdf(TransferWithDetailsResponseDto transfer, string storeType, string storeName, string qrUrl);
        byte[] GenerateListPdf<T>(IEnumerable<T> data, List<(string ColumnName, string PropertyName)> columns, string title, string subtitle = "") where T : class;
    }
}
