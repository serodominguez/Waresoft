using Dapper;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Product;
using Infrastructure.Persistences.Projections;
using Infrastructure.Persistences.ReadModels.Product;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Persistences.Repositories.Product
{
    public class ProductQueryRepository : IProductQueryRepository
    {
        private readonly DbContextSystem _context;

        public ProductQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<ProductReadModel> GetProductsQueryable()
        {
            return _context.Product
                .AsTracking()
                .Where(p => p.AuditDeleteUser == null && p.AuditDeleteDate == null)
                .Select(ProductProjection.ToSummary);
        }

        public IQueryable<ProductReadModel> GetProductByIdQueryable(int productId)
        {
            return _context.Product
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(ProductProjection.ToSummary);
        }

        public IQueryable<ProductSelectReadModel> GetProductsSelectQueryable()
        {
            return _context.Product
                .AsNoTracking()
                .Where(p => p.AuditDeleteUser == null && p.AuditDeleteDate == null)
                .Select(ProductProjection.ToSelect);
        }

        public async Task<ProductStatsReadModel> GetProductStatsAsync(CancellationToken cancellationToken)
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

            return await connection.QuerySingleAsync<ProductStatsReadModel>(command);
        }
    } 
}
