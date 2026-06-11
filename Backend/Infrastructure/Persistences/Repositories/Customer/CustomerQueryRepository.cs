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
    }
}
