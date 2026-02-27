using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsReceiptDetailsRepository
    {
        IQueryable<GoodsReceiptDetailsEntity> GetGoodsReceiptDetailsQueryable(int receiptId);
        IQueryable<GoodsReceiptDetailsEntity> GetGoodsReceiptDetailsByProductQueryable(int storeId, int productId);
    }
}
