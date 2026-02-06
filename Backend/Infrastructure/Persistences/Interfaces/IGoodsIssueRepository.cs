using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsIssueRepository
    {
        Task<string> GenerateCodeAsync();
        IQueryable<GoodsIssueEntity> GetGoodsIssueQueryableByStore(int storeId);
        Task<GoodsIssueEntity?> GetGoodsIssueByIdAsync(int issueId);
        Task<bool> RegisterGoodsIssueAsync(GoodsIssueEntity entity);
        Task<bool> CancelGoodsIssueAsync(GoodsIssueEntity entity);
    }
}
