using Dapper;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Customer;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Customer;
using Infrastructure.Persistences.ReadModels.Product;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Persistences.Repositories.Customer
{
    public class CustomerQueryRepository : ICustomerQueryRepository
    {
        private readonly DbContextSystem _context;

        public CustomerQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<CustomerReadModel> GetCustomersListQueryable()
        {
            return _context.Customer
                .AsNoTracking()
                .Where(c => c.AuditDeleteUser == null && c.AuditDeleteDate == null)
                .Select(CustomerProjection.ToSummary);
        }

        public IQueryable<CustomerReadModel> GetCustomerByIdQueryable(int customerId)
        {
            return _context.Customer
                .AsNoTracking()
                .Where(c => c.Id == customerId)
                .Select(CustomerProjection.ToSummary);
        }

        public IQueryable<CustomerSelectReadModel> GetCustomersSelectQueryable()
        {
            return _context.Customer
                .AsNoTracking()
                .Where(c => c.AuditDeleteUser == null && c.AuditDeleteDate == null)
                .Select(CustomerProjection.ToSelect);
        }

        public async Task<CustomerStatsReadModel> GetCustomerStatsAsync(CancellationToken cancellationToken)
        {
            var today = DateTime.Now;
            var startOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
            var startOfPreviousMonth = startOfCurrentMonth.AddMonths(-1);

            var sql = @"
                        SELECT
                            COUNT(CASE WHEN IsActive = 1 THEN 1 END) AS TotalActive,
                            COUNT(CASE WHEN AuditCreateDate >= @StartOfCurrentMonth THEN 1 END) AS NewThisMonth,
                            COUNT(CASE WHEN AuditCreateDate >= @StartOfPreviousMonth 
                                        AND AuditCreateDate <  @StartOfCurrentMonth THEN 1 END) AS NewPreviousMonth
                        FROM CUSTOMERS
                        WHERE AuditDeleteUser IS NULL
                        AND AuditDeleteDate IS NULL";

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                new { StartOfCurrentMonth = startOfCurrentMonth, StartOfPreviousMonth = startOfPreviousMonth },
                cancellationToken: cancellationToken
            );

            return await connection.QuerySingleAsync<CustomerStatsReadModel>(command);
        }
    }
}
