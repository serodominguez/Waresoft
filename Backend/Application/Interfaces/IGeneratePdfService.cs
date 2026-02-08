using Application.Dtos.Response.GoodsIssue;
using Application.Dtos.Response.GoodsReceipt;
using Application.Dtos.Response.StoreInventory;
using Application.Dtos.Response.Transfer;

namespace Application.Interfaces
{
    public interface IGeneratePdfService
    {
        byte[] GoodsIssueGeneratePdf(GoodsIssueWithDetailsResponseDto issue);
        byte[] GoodsReceiptGeneratePdf(GoodsReceiptWithDetailsResponseDto receipt);
        byte[] TransferGeneratePdf(TransferWithDetailsResponseDto transfer);
        byte[] InventoryGeneratePdf(List<StoreInventoryResponseDto> inventory, string storeName);
        byte[] GenerateListPdf<T>(IEnumerable<T> data, List<(string ColumnName, string PropertyName)> columns, string title, string subtitle = "") where T : class;
    }
}
