using Infrastructure.Persistences.ReadModels.GoodsIssue;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsIssueQueryRepository
    {
        IQueryable<GoodsIssueReadModel> GetGoodsIssueQueryableByStore(int storeId);
        IQueryable<GoodsIssueWithDetailsReadModel> GetGoodsIssueByIdAsQueryable(int issueId);

    }
}
