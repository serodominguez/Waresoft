using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGoodsIssueDetailsRepository
    {
        Task<IEnumerable<GoodsIssueDetailsEntity>> GetGoodsIssueDetailsAsync(int issueId);
        Task<IEnumerable<GoodsIssueDetailsEntity>> GetGoodsIssueDetailsByProductAsync(int storeId, int productId);
    }
}
