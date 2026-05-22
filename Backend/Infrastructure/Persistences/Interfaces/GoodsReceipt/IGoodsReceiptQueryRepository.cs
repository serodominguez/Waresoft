using Infrastructure.Persistences.ReadModels.GoodsReceipt;

namespace Infrastructure.Persistences.Interfaces.GoodsReceipt
{
    public interface IGoodsReceiptQueryRepository
    {
        IQueryable<GoodsReceiptReadModel> GetGoodsReceiptQueryableByStore(int storeId);
        IQueryable<GoodsReceiptWithDetailsReadModel> GetGoodsReceiptByIdAsQueryable(int receiptId);
    }
}
