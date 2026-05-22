using Infrastructure.Persistences.ReadModels.GoodsIssue;

namespace Infrastructure.Persistences.Interfaces.GoodsIssue
{
    public interface IGoodsIssueQueryRepository
    {
        IQueryable<GoodsIssueReadModel> GetGoodsIssueQueryableByStore(int storeId);
        IQueryable<GoodsIssueWithDetailsReadModel> GetGoodsIssueByIdAsQueryable(int issueId);

    }
}
