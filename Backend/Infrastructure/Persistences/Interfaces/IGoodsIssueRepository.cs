using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsIssueRepository
    {
        IQueryable<GoodsIssueEntity> GetGoodsIssueQueryableByStore(int storeId);
        IQueryable<GoodsIssueEntity> GetGoodsIssueByIdAsQueryable(int issueId);
        Task AddGoodsIssueAsync(GoodsIssueEntity entity);
    }
}
