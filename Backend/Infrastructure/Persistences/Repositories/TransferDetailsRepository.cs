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
                    .Where(d => d.IdTransfer == transferId)
                    .OrderBy(d => d.Item)
                    .Select(d => new TransferDetailsEntity
                    {
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

        public IQueryable<TransferDetailsEntity> GetTransferDetailsByProductQueryable(int storeId, int productId)
        {
            return _context.TransferDetails
                .Include(d => d.Transfer)
                .Where(d => d.IdProduct == productId && (d.Transfer.IdStoreOrigin == storeId || d.Transfer.IdStoreDestination == storeId));
        }
    }
}
