using Infrastructure.Persistences.ReadModels.Supplier;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ISupplierQueryRepository
    {
        IQueryable<SupplierReadModel> GetSuppliersListQueryable();
        IQueryable<SupplierReadModel> GetSupplierByIdQueryable(int supplierId);
        IQueryable<SupplierSelectReadModel> GetSuppliersSelectQueryable();
    }
}
