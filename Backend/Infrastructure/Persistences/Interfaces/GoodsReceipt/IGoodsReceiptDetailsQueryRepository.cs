using Infrastructure.Persistences.ReadModels.GoodsReceipt;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsReceiptDetailsQueryRepository
    {
        IQueryable<GoodsReceiptDetailsReadModel> GetGoodsReceiptDetailsQueryable(int receiptId);
    }
}
