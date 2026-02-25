using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsIssueRepository
    {
        Task<string> GenerateCodeAsync();
        IQueryable<GoodsIssueEntity> GetGoodsIssueQueryableByStore(int storeId);
        IQueryable<GoodsIssueEntity> GetGoodsIssueByIdAsQueryable(int issueId);
        Task AddGoodsIssueAsync(GoodsIssueEntity entity);
    }
}
