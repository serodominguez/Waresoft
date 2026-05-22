using Infrastructure.Persistences.ReadModels.GoodsIssue;

namespace Infrastructure.Persistences.Interfaces.GoodsIssue
{
    public interface IGoodsIssueDetailsQueryRepository
    {
        IQueryable<GoodsIssueDetailsReadModel> GetGoodsIssueDetailsQueryable(int issueId);
    }
}
