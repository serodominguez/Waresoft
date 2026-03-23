using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class GoodsIssueDetailsRepository : IGoodsIssueDetailsRepository
    {
        private readonly DbContextSystem _context;

        public GoodsIssueDetailsRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<GoodsIssueDetailsEntity> GetGoodsIssueDetailsQueryable(int issueId)
        {
            return _context.GoodsIssueDetails
                    .Where(d => d.IdIssue == issueId)
                    .OrderBy(d => d.Item)
                    .Select(d => new GoodsIssueDetailsEntity
                    {
                        IdIssue = d.IdIssue,
                        Item = d.Item,
                        IdProduct = d.IdProduct,
                        Product = new ProductEntity
                        {
                            Id = d.Product.Id,
                            Code = d.Product.Code,
                            Description = d.Product.Description,
                            Material = d.Product.Material,
                            Color = d.Product.Color,
                            Brand = new BrandEntity
                            {
                                Id = d.Product.Brand.Id,
                                BrandName = d.Product.Brand.BrandName
                            },
                            Category = new CategoryEntity
                            {
                                Id = d.Product.Category.Id,
                                CategoryName = d.Product.Category.CategoryName
                            }
                        },
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        TotalPrice = d.TotalPrice,
                    });
        }

        public IQueryable<GoodsIssueDetailsEntity> GetGoodsIssueDetailsByProductQueryable(int storeId, int productId)
        {
            return _context.GoodsIssueDetails
                    .Where(d => d.IdProduct == productId && d.GoodsIssue.IdStore == storeId)
                    .Select(d => new GoodsIssueDetailsEntity
                    {
                        IdIssue = d.IdIssue,
                        Item = d.Item,
                        IdProduct = d.IdProduct,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        TotalPrice = d.TotalPrice,
                        GoodsIssue = new GoodsIssueEntity
                        {
                            IdIssue = d.GoodsIssue.IdIssue,
                            Code = d.GoodsIssue.Code,
                            Type = d.GoodsIssue.Type,
                            TotalAmount = d.GoodsIssue.TotalAmount,
                            Annotations = d.GoodsIssue.Annotations,
                            IdUser = d.GoodsIssue.IdUser,
                            IdStore = d.GoodsIssue.IdStore,
                            AuditCreateDate = d.GoodsIssue.AuditCreateDate,
                            Status = d.GoodsIssue.Status,
                            IsActive = d.GoodsIssue.IsActive
                        },
                    });
        }
    }
}
