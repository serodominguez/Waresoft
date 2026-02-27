using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                .Include(d => d.GoodsIssue)
                .Where(d => d.IdProduct == productId && d.GoodsIssue.IdStore == storeId);
        }
    }
}
