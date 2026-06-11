using Dapper;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Dashboard;
using Infrastructure.Persistences.ReadModels.Dashboard;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Persistences.Repositories.Dashboard
{
    public class DashboardQueryRepository : IDashboardQueryRepository
    {
        private readonly DbContextSystem _context;

        public DashboardQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<DashboardGoodsIssueStatsReadModel> GetGoodsIssueStatsLast14DaysAsync(int storeId, CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;
            var last7Days = today.AddDays(-7);
            var previous7Days = today.AddDays(-14);

            var sql = @"
                        SELECT
                            COUNT(*) AS TotalIssues,
                            COUNT(CASE WHEN AuditCreateDate >= @Last7Days THEN 1 END) AS IssuedLast7Days,
                            COUNT(CASE WHEN AuditCreateDate >= @Previous7Days 
                            AND AuditCreateDate < @Last7Days THEN 1 END) AS IssuedPrevious7Days
                        FROM GOODS_ISSUE
                        WHERE IdStore = @StoreId
                        AND AuditDeleteUser IS NULL
                        AND AuditDeleteDate IS NULL";

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { StoreId = storeId, Last7Days = last7Days, Previous7Days = previous7Days },
                cancellationToken: cancellationToken
            );

            return await connection.QuerySingleAsync<DashboardGoodsIssueStatsReadModel>(command);
        }

        public async Task<DashboardStoreInventoryStatsReadModel> GetInventoryStatsCurrentMonthAsync(int storeId, CancellationToken cancellationToken)
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);

            var sql = @"
                        SELECT
                            COUNT(CASE WHEN si.StockAvailable < si.MinimumStock THEN 1 END) AS BelowMinimum,
                            COUNT(CASE WHEN si.StockAvailable < si.MinimumStock AND si.AuditUpdateDate >= @StartOfMonth THEN 1 END) AS BelowMinimumThisMonth,
                            COUNT(CASE WHEN si.StockAvailable < si.MinimumStock AND si.AuditUpdateDate >= @StartOfLastMonth 
                            AND si.AuditUpdateDate <  @StartOfMonth THEN 1 END) AS BelowMinimumLastMonth
                        FROM STORES_INVENTORY si INNER JOIN PRODUCTS p ON p.IdProduct = si.IdProduct
                        WHERE si.IdStore = @StoreId AND p.IsActive = 1 AND p.AuditDeleteUser IS NULL AND p.AuditDeleteDate IS NULL";

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { StoreId = storeId, StartOfMonth = startOfMonth, StartOfLastMonth = startOfLastMonth },
                cancellationToken: cancellationToken
            );

            return await connection.QuerySingleAsync<DashboardStoreInventoryStatsReadModel>(command);
        }

        public async Task<IEnumerable<DashboardMovementStatsReadModel>> GetMovementsLast6MonthsAsync(int storeId, CancellationToken cancellationToken)
        {
            var today = DateTime.Now;
            var startDate = new DateTime(today.Year, today.Month, 1).AddMonths(-5);

            var sql = @"
                        SELECT m.Month,m.Year,
                                ISNULL(r.TotalReceipts, 0) AS TotalReceipts,
                                ISNULL(i.TotalIssues,   0) AS TotalIssues
                        FROM (SELECT MONTH(DATEADD(MONTH, number, @StartDate)) AS Month,
                                YEAR(DATEADD(MONTH,  number, @StartDate)) AS Year
                                FROM master.dbo.spt_values
                                WHERE type = 'P' AND number BETWEEN 0 AND 5) m
                        LEFT JOIN (SELECT MONTH(AuditCreateDate) AS Month,
                                YEAR(AuditCreateDate)  AS Year,
                                SUM(TotalAmount) AS TotalReceipts
                        FROM GOODS_RECEIPT
                        WHERE IdStore = @StoreId AND AuditCreateDate >= @StartDate AND AuditDeleteUser IS NULL AND AuditDeleteDate IS NULL
                        GROUP BY MONTH(AuditCreateDate), YEAR(AuditCreateDate)) r ON r.Month = m.Month AND r.Year = m.Year
                        LEFT JOIN (SELECT MONTH(AuditCreateDate) AS Month, YEAR(AuditCreateDate)  AS Year, SUM(TotalAmount) AS TotalIssues
                        FROM GOODS_ISSUE
                        WHERE IdStore = @StoreId AND AuditCreateDate >= @StartDate AND AuditDeleteUser IS NULL AND AuditDeleteDate IS NULL
                        GROUP BY MONTH(AuditCreateDate), YEAR(AuditCreateDate)) i ON i.Month = m.Month AND i.Year = m.Year
                        ORDER BY m.Year, m.Month";

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { StoreId = storeId, StartDate = startDate },
                cancellationToken: cancellationToken
            );

            return await connection.QueryAsync<DashboardMovementStatsReadModel>(command);

        }

        public async Task<DashboardProductReplenishmentReadModel> GetProductReplenishmentAsync(int storeId, CancellationToken cancellationToken)
        {
            var sql = @"
                        SELECT
                            COUNT(CASE WHEN p.Replenishment = 1 AND p.IsActive = 1 THEN 1 END) AS Available,
                            COUNT(CASE WHEN p.Replenishment = 2 AND p.IsActive = 0 THEN 1 END) AS NotAvailable,
                            COUNT(CASE WHEN p.AuditDeleteUser IS NOT NULL THEN 1 END) AS Discontinued
                        FROM STORES_INVENTORY si
                        INNER JOIN PRODUCTS p ON p.IdProduct = si.IdProduct
                        WHERE si.IdStore = @StoreId";

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { StoreId = storeId },
                cancellationToken: cancellationToken
            );

            return await connection.QuerySingleAsync<DashboardProductReplenishmentReadModel>(command);
        }

        public async Task<DashboardProductStatsReadModel> GetProductStatsCurrentMonthAsync(CancellationToken cancellationToken)
        {
            var hoy = DateTime.Now;
            var startOfCurrentMonth = new DateTime(hoy.Year, hoy.Month, 1);

            var sql = @"
                        SELECT
                            COUNT(CASE WHEN IsActive = 1 THEN 1 END) AS TotalActive,
                            COUNT(CASE WHEN AuditCreateDate >= @StartOfCurrentMonth THEN 1 END)  AS NewThisMonth
                        FROM PRODUCTS
                        WHERE AuditDeleteUser IS NULL
                        AND AuditDeleteDate IS NULL";

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { StartOfCurrentMonth = startOfCurrentMonth },
                cancellationToken: cancellationToken
            );

            return await connection.QuerySingleAsync<DashboardProductStatsReadModel>(command);
        }

        public async Task<IEnumerable<DashboardTransferByStoreReadModel>> GetTransfersByStoreLast6MonthsAsync(int storeId, CancellationToken cancellationToken)
        {
            var startDate = DateTime.Now.AddMonths(-6);

            var sql = @"
                        SELECT TOP 5 s.StoreName,
                            COUNT(t.IdStoreDestination) AS TotalTransfers
                        FROM TRANSFERS t
                        INNER JOIN STORES s ON s.IdStore = t.IdStoreDestination
                        WHERE t.IdStoreOrigin = @StoreId
                        AND t.AuditCreateDate >= @StartDate
                        AND t.AuditDeleteUser IS NULL
                        AND t.AuditDeleteDate IS NULL
                        GROUP BY s.StoreName
                        ORDER BY TotalTransfers DESC";

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { StoreId = storeId, StartDate = startDate },
                cancellationToken: cancellationToken
            );

            return await connection.QueryAsync<DashboardTransferByStoreReadModel>(command);
        }

        public async Task<DashboardTransferPendingReadModel> GetTransferPendingAsync(int storeId, CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;
            var yesterday = today.AddDays(-1);
            var startOfThisMonth = new DateTime(today.Year, today.Month, 1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);

            var sql = @"
                        SELECT
                            COUNT(CASE WHEN Status = 1 AND IdStoreDestination = @StoreId THEN 1 END) AS TotalPending,
                            COUNT(CASE WHEN Status = 1 AND IdStoreDestination = @StoreId 
                            AND CAST(AuditCreateDate AS DATE) = @Today THEN 1 END) AS PendingToday,
                            COUNT(CASE WHEN Status = 1 AND IdStoreDestination = @StoreId 
                            AND CAST(AuditCreateDate AS DATE) = @Yesterday THEN 1 END) AS PendingYesterday,
                            COUNT(CASE WHEN Status = 1 AND IdStoreOrigin = @StoreId THEN 1 END) AS TotalSent,
                            COUNT(CASE WHEN Status = 1 AND IdStoreOrigin = @StoreId 
                            AND AuditCreateDate >= @StartOfThisMonth THEN 1 END) AS SentThisMonth,
                            COUNT(CASE WHEN Status = 1 AND IdStoreOrigin = @StoreId 
                            AND AuditCreateDate >= @StartOfLastMonth 
                            AND AuditCreateDate < @StartOfThisMonth THEN 1 END) AS SentLastMonth
                        FROM TRANSFERS
                        WHERE AuditDeleteUser IS NULL
                        AND AuditDeleteDate IS NULL";

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new
                {
                    StoreId = storeId,
                    Today = today,
                    Yesterday = yesterday,
                    StartOfThisMonth = startOfThisMonth,
                    StartOfLastMonth = startOfLastMonth
                },
                cancellationToken: cancellationToken
            );

            return await connection.QuerySingleAsync<DashboardTransferPendingReadModel>(command);
        }

        public async Task<IEnumerable<DashboardTransferStatusReadModel>> GetTransferStatusLast6MonthsAsync(int storeId, CancellationToken cancellationToken)
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-5);

            var sql = @"
                        SELECT m.Month, m.Year,
                            COUNT(CASE WHEN t.Status = 1 AND t.IdStoreOrigin      = @StoreId THEN 1 END) AS Sent,
                            COUNT(CASE WHEN t.Status = 1 AND t.IdStoreDestination = @StoreId THEN 1 END) AS Pending,
                            COUNT(CASE WHEN t.Status = 2 AND t.IdStoreDestination = @StoreId THEN 1 END) AS Received
                        FROM (SELECT MONTH(DATEADD(MONTH, number, @StartDate)) AS Month, YEAR(DATEADD(MONTH,  number, @StartDate)) AS Year
                                FROM master.dbo.spt_values WHERE type = 'P' AND number BETWEEN 0 AND 5) m
                        LEFT JOIN TRANSFERS t ON MONTH(t.AuditCreateDate) = m.Month
                            AND YEAR(t.AuditCreateDate)  = m.Year
                            AND (t.IdStoreOrigin = @StoreId OR t.IdStoreDestination = @StoreId)
                            AND t.AuditDeleteUser IS NULL
                            AND t.AuditDeleteDate IS NULL
                        GROUP BY m.Month, m.Year
                        ORDER BY m.Year, m.Month";

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { StoreId = storeId, StartDate = startDate },
                cancellationToken: cancellationToken
            );

            return await connection.QueryAsync<DashboardTransferStatusReadModel>(command);
        }
    }
}
