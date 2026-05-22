using Infrastructure.Persistences.ReadModels.GoodsReceipt;

namespace Infrastructure.Persistences.Interfaces.GoodsReceipt
{
    public interface IGoodsReceiptDetailsQueryRepository
    {
        IQueryable<GoodsReceiptDetailsReadModel> GetGoodsReceiptDetailsQueryable(int receiptId);
    }
}
