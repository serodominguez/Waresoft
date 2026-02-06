using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsReceiptRepository
    {
        Task<string> GenerateCodeAsync();
        IQueryable<GoodsReceiptEntity> GetGoodsReceiptQueryableByStore(int storeId);
        Task<GoodsReceiptEntity?> GetGoodsReceiptByIdAsync(int receiptId);
        Task<bool> RegisterGoodsReceiptAsync(GoodsReceiptEntity entity);
        Task<bool> CancelGoodsReceiptAsync(GoodsReceiptEntity entity);
    }
}
