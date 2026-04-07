using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GoodsReceiptRepository : IGoodsReceiptRepository
    {
        private readonly DbContextSystem _context;

        public GoodsReceiptRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<GoodsReceiptEntity> GetGoodsReceiptQueryableByStore(int storeId)
        {
            return _context.GoodsReceipt
                .Where(r => r.IdStore == storeId)
                .Select(r => new GoodsReceiptEntity
                {
                    IdReceipt = r.IdReceipt,
                    Code = r.Code,
                    Type = r.Type,
                    DocumentDate = r.DocumentDate,
                    DocumentType = r.DocumentType,
                    DocumentNumber = r.DocumentNumber,
                    TotalAmount = r.TotalAmount,
                    Annotations = r.Annotations,
                    IdSupplier = r.IdSupplier,
                    Supplier = r.Supplier != null ? new SupplierEntity
                    {
                        Id = r.Supplier.Id,
                        CompanyName = r.Supplier.CompanyName
                    } : null,
                    IdStore = r.IdStore,
                    Store = new StoreEntity
                    {
                        Id = r.Store.Id,
                        StoreName = r.Store.StoreName
                    },
                    AuditCreateUser = r.AuditCreateUser,
                    AuditCreateDate = r.AuditCreateDate,
                    Status = r.Status
                });
        }

        public IQueryable<GoodsReceiptEntity> GetGoodsReceiptByIdAsQueryable(int receiptId)
        {
            return _context.GoodsReceipt
                .Where(r => r.IdReceipt == receiptId)
                .Include(r => r.Supplier)
                .Include(r => r.Store);
        }

        public async Task AddGoodsReceiptAsync(GoodsReceiptEntity entity)
        {
            await _context.AddAsync(entity);
        }
    }
}
