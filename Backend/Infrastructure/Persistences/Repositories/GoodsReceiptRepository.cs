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
                // Buscar la secuencia
                var sequence = await _context.Sequence
                    .FirstOrDefaultAsync(s => s.Name == "GOODS_RECEIPT");

                if (sequence == null)
                {
                    // Si no existe, crearla (por si no se ejecutó la migración con HasData)
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
                    // Incrementar el valor
                    sequence.CurrentValue++;
                    sequence.LastUpdated = DateTime.Now;
                    _context.Sequence.Update(sequence);
                }

                // Guardar cambios
                await _context.SaveChangesAsync();

                // Formatear el código con ceros a la izquierda (opcional)
                return $"ENT-{sequence.CurrentValue.ToString().PadLeft(6, '0')}";
                // Resultado: ENT-000001, ENT-000002, etc.

                // O sin ceros: return $"ENT-{sequence.CurrentValue}";
                // Resultado: ENT-1, ENT-2, etc.
        }

        public IQueryable<GoodsReceiptEntity> GetGoodsReceiptQueryableByStore(int storeId)
        {
            return _context.GoodsReceipt
                .AsNoTracking()
                .Where(r => r.IdStore == storeId)
                .Include(r => r.Supplier)
                .Include(r => r.Store);
        }

        public async Task<GoodsReceiptEntity?> GetGoodsReceiptByIdAsync(int receiptId)
        {
            var entity = await _context.GoodsReceipt
                .AsNoTracking()
                .Include(r => r.Supplier)
                .Include(r => r.Store)
                .FirstOrDefaultAsync(x => x.IdReceipt.Equals(receiptId));
            
            return entity;
        }

        public async Task<bool> RegisterGoodsReceiptAsync(GoodsReceiptEntity entity)
        {
            await _context.AddAsync(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> CancelGoodsReceiptAsync(GoodsReceiptEntity entity)
        {
            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
