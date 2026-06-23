using Infrastructure.Persistences.ReadModels.GoodsIssue;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsIssueDetailsQueryRepository
    {
        IQueryable<GoodsIssueDetailsReadModel> GetGoodsIssueDetailsQueryable(int issueId);
    }
}
