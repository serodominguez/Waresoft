using Dapper;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.InventoryPeriod;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.InventoryPeriod;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.InventoryPeriod 
{
    public class InventoryPeriodQueryRepository : IInventoryPeriodQueryRepository
    {
        private readonly DbContextSystem _context;
        private readonly string _connectionString;

        public InventoryPeriodQueryRepository(DbContextSystem context)
        {
            _context = context;
            _connectionString = context.Database.GetConnectionString()!;
        }

        public IQueryable<InventoryPeriodReadModel> GetPeriodListQueryable(int storeId)
        {
            return _context.InventoryPeriod
                .AsNoTracking()
                .Where(p => p.IdStore == storeId)
                .OrderByDescending(p => p.StartDate)
                .Select(InventoryPeriodProjection.ToSummary);
        }

        public async Task<InventoryPeriodDetailReadModel?> GetPeriodDetailAsync(int periodId)
        {
            return await _context.InventoryPeriod
                .AsNoTracking()
                .Where(p => p.IdPeriod == periodId)
                .Select(InventoryPeriodProjection.ToDetail)
                .FirstOrDefaultAsync();
        }


        public async Task<InventoryPeriodOpeningReadModel?> GetOpeningByPeriodAsync(int periodId)
        {
            return await _context.InventoryPeriod
                .AsNoTracking()
                .Where(p => p.IdPeriod == periodId)
                .Select(InventoryPeriodProjection.ToOpeningSummary)
                .FirstOrDefaultAsync();
        }

        public async Task<InventoryPeriodClosingReadModel?> GetClosingByPeriodAsync(int periodId)
        {
            return await _context.InventoryPeriod
                .AsNoTracking()
                .Where(p => p.IdPeriod == periodId)
                .Select(InventoryPeriodProjection.ToClosingSummary)
                .FirstOrDefaultAsync();
        }

        public async Task<List<InventoryPeriodOpeningItemReadModel>> GetOpeningItemByPeriodAsync(int periodId)
        {
            return await _context.InventoryPeriodOpening
                .AsNoTracking()
                .Where(o => o.IdPeriod == periodId)
                .Select(InventoryPeriodProjection.ToOpeningItem)
                .ToListAsync();
        }

        public async Task<List<InventoryPeriodClosingItemReadModel>> GetClosingItemByPeriodAsync(int periodId)
        {
            return await _context.InventoryPeriodClosing
                .AsNoTracking()
                .Where(c => c.IdPeriod == periodId)
                .Select(InventoryPeriodProjection.ToClosingItem)
                .ToListAsync();
        }

        public async Task<List<InventoryPeriodClosingItemReadModel>> CalculateSystemStockAsync(int periodId)
        {
            const string sql = @"
                            SELECT  @PeriodId AS IdPeriod,
                                    p.IdProduct,
                                    p.Code AS ProductCode,
                                    p.Description AS ProductDescription,
                                    p.UnitMeasure,
                                    op.OpeningStock,
                                    op.OpeningStock + ISNULL(SUM(CASE WHEN m.MovementType = 'Entrada' THEN m.Quantity
                                WHEN m.MovementType = 'Salida'  THEN -m.Quantity
                                ELSE 0 END), 0) AS SystemStock
                            FROM InventoryPeriodOpening op
                            INNER JOIN Products p ON p.IdProduct = op.IdProduct
                            LEFT JOIN (
                            -- Entradas
                            SELECT d.IdProduct, d.Quantity, 'Entrada' AS MovementType
                            FROM GoodsReceiptDetails d
                            INNER JOIN GoodsReceipt r ON r.IdReceipt = d.IdReceipt
                            WHERE r.IdPeriod = @PeriodId
                            AND r.IsActive = 1
                            AND r.Status = 1
                            UNION ALL
                            -- Salidas
                            SELECT d.IdProduct, d.Quantity, 'Salida' AS MovementType
                            FROM GoodsIssueDetails d
                            INNER JOIN GoodsIssue i ON i.IdIssue = d.IdIssue
                            WHERE i.IdPeriod = @PeriodId
                            AND i.IsActive = 1
                            AND i.Status = 1
                            UNION ALL
                            -- Entradas por transferencia (destino)
                            SELECT d.IdProduct, d.Quantity, 'Entrada' AS MovementType
                            FROM TransfersDetails d
                            INNER JOIN Transfers t ON t.IdTransfer = d.IdTransfer
                            INNER JOIN InventoryPeriods ip ON ip.IdPeriod = @PeriodId
                            WHERE t.IdPeriod = @PeriodId
                            AND t.IdStoreDestination = ip.IdStore
                            AND t.IsActive = 1
                            AND t.Status != 0
                            UNION ALL
                            -- Salidas por transferencia (origen)
                            SELECT d.IdProduct, d.Quantity, 'Salida' AS MovementType
                            FROM TransfersDetails d
                            INNER JOIN Transfers t ON t.IdTransfer = d.IdTransfer
                            INNER JOIN InventoryPeriods ip ON ip.IdPeriod = @PeriodId
                            WHERE t.IdPeriod = @PeriodId
                            AND t.IdStoreOrigin = ip.IdStore
                            AND t.IsActive = 1
                            AND t.Status != 0) m ON m.IdProduct = op.IdProduct
                            WHERE op.IdPeriod = @PeriodId
                            GROUP BY p.IdProduct, p.Code, p.Description, p.UnitMeasure, op.OpeningStock
                            ORDER BY p.Description ASC";

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<InventoryPeriodClosingItemReadModel>(
                sql, new { PeriodId = periodId });
            return result.ToList();
        }
    }
}
