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

        public async Task<string> GenerateCodeAsync()
        {
                var sequence = await _context.Sequence
                    .FirstOrDefaultAsync(s => s.Name == "GOODS_RECEIPT");

                if (sequence == null)
                {
                    sequence = new SequenceEntity
                    {
                        Name = "GOODS_RECEIPT",
                        CurrentValue = 1,
                        LastUpdated = DateTime.Now
                    };
                    await _context.Sequence.AddAsync(sequence);
                }
                else
                {
                    sequence.CurrentValue++;
                    sequence.LastUpdated = DateTime.Now;
                    _context.Sequence.Update(sequence);
                }

                await _context.SaveChangesAsync();

                return $"ENT-{sequence.CurrentValue.ToString().PadLeft(6, '0')}";
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
                    Supplier = new SupplierEntity
                    {
                        Id = r.Supplier.Id,
                        CompanyName = r.Supplier.CompanyName
                    },
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
