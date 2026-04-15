using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsIssueDetailsRepository
    {
        IQueryable<GoodsIssueDetailsEntity> GetGoodsIssueDetailsQueryable(int issueId);
    }
}
