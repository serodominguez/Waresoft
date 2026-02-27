using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsReceiptRepository
    {
        Task<string> GenerateCodeAsync();
        IQueryable<GoodsReceiptEntity> GetGoodsReceiptQueryableByStore(int storeId);
        IQueryable<GoodsReceiptEntity> GetGoodsReceiptByIdAsQueryable(int receiptId);
        Task AddGoodsReceiptAsync(GoodsReceiptEntity entity);
    }
}
