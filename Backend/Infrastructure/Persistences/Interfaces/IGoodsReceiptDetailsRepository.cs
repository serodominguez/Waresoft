using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsReceiptDetailsRepository
    {
        Task<IEnumerable<GoodsReceiptDetailsEntity>> GetGoodsReceiptDetailsAsync(int receiptId);
        Task<IEnumerable<GoodsReceiptDetailsEntity>> GetGoodsReceiptDetailsByProductAsync(int storeId, int productId);
    }
}
