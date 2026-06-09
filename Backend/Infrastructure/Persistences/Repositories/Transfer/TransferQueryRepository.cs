using Dapper;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Transfer;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Product;
using Infrastructure.Persistences.ReadModels.Transfer;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Persistences.Repositories.Transfer
{
    public class TransferQueryRepository : ITransferQueryRepository
    {
        private readonly DbContextSystem _context;

        public TransferQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<TransferReadModel> GetTransferQueryableByStore(int storeId)
        {
            return _context.Transfer
                .Where(t => t.IdStoreOrigin == storeId || t.IdStoreDestination == storeId)
                .Select(TransferProjection.ToSummary);
        }

        public IQueryable<TransferWithDetailsReadModel> GetTransferByIdAsQueryable(int transferId)
        {
            return _context.Transfer
                 .AsNoTracking()
                 .Where(t => t.Id == transferId)
                 .Select(TransferProjection.ToWithDetails);
        }

        public async Task<TransferStatsReadModel> GetTransferStatsAsync(int storeId, CancellationToken cancellationToken)
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
                new {
                        StoreId = storeId,
                        Today = today,
                        Yesterday = yesterday,
                        StartOfThisMonth = startOfThisMonth,
                        StartOfLastMonth = startOfLastMonth
                    },
                cancellationToken: cancellationToken
            );

            return await connection.QuerySingleAsync<TransferStatsReadModel>(command);

        }
    }
}
