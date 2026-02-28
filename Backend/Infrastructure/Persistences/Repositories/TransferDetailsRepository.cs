using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Persistences.Repositories
{
    public class TransferDetailsRepository : ITransferDetailsRepository
    {
        private readonly DbContextSystem _context;

        public TransferDetailsRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<TransferDetailsEntity> GetTransferDetailsQueryable(int transferId)
        {
            return _context.TransferDetails
                    .Where(t => t.IdTransfer == transferId)
                    .OrderBy(t => t.Item)
                    .Select(t => new TransferDetailsEntity
                    {
                        IdTransfer = t.IdTransfer,
                        Item = t.Item,
                        IdProduct = t.IdProduct,
                        Product = new ProductEntity
                        {
                            Id = t.Product.Id,
                            Code = t.Product.Code,
                            Description = t.Product.Description,
                            Material = t.Product.Material,
                            Color = t.Product.Color,
                            Brand = new BrandEntity
                            {
                                Id = t.Product.Brand.Id,
                                BrandName = t.Product.Brand.BrandName
                            },
                            Category = new CategoryEntity
                            {
                                Id = t.Product.Category.Id,
                                CategoryName = t.Product.Category.CategoryName
                            }
                        },
                        Quantity = t.Quantity,
                        UnitPrice = t.UnitPrice,
                        TotalPrice = t.TotalPrice,
                    });
        }

        public IQueryable<TransferDetailsEntity> GetTransferDetailsByProductQueryable(int storeId, int productId)
        {
            return _context.TransferDetails
                .Include(d => d.Transfer)
                .Where(d => d.IdProduct == productId && (d.Transfer.IdStoreOrigin == storeId || d.Transfer.IdStoreDestination == storeId));
        }
    }
}
