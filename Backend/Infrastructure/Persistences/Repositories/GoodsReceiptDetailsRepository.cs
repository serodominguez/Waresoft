using DocumentFormat.OpenXml.Bibliography;
using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GoodsReceiptDetailsRepository : IGoodsReceiptDetailsRepository
    {
        private readonly DbContextSystem _context;

        public GoodsReceiptDetailsRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<GoodsReceiptDetailsEntity> GetGoodsReceiptDetailsQueryable(int receiptId)
        {
            return _context.GoodsReceiptDetails
                    .Where(r => r.IdReceipt == receiptId)
                    .OrderBy(r => r.Item)
                    .Select(r => new GoodsReceiptDetailsEntity
                    {
                        IdReceipt = r.IdReceipt,
                        Item = r.Item,
                        IdProduct = r.IdProduct,
                        Product = new ProductEntity
                        {
                            Id = r.Product.Id,
                            Code = r.Product.Code,
                            Description = r.Product.Description,
                            Material = r.Product.Material,
                            Color = r.Product.Color,
                            Brand = new BrandEntity
                            {
                                Id = r.Product.Brand.Id,
                                BrandName = r.Product.Brand.BrandName
                            },
                            Category = new CategoryEntity
                            {
                                Id = r.Product.Category.Id,
                                CategoryName = r.Product.Category.CategoryName
                            }
                        },
                        Quantity = r.Quantity,
                        UnitCost = r.UnitCost,
                        TotalCost = r.TotalCost,
                    });
        }

        public IQueryable<GoodsReceiptDetailsEntity> GetGoodsReceiptDetailsByProductQueryable(int storeId, int productId)
        {
            return _context.GoodsReceiptDetails
                .Include(d => d.GoodsReceipt)
                .Where(d => d.IdProduct == productId && d.GoodsReceipt.IdStore == storeId);
        }
    }
}
