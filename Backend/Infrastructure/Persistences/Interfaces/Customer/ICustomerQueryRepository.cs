using Infrastructure.Persistences.ReadModels.Customer;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ICustomerQueryRepository
    {
        IQueryable<CustomerReadModel> GetCustomersListQueryable();
        IQueryable<CustomerReadModel> GetCustomerByIdQueryable(int customerId);
        IQueryable<CustomerSelectReadModel> GetCustomersSelectQueryable();
    }
}
