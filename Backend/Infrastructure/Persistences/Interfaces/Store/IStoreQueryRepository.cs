using Infrastructure.Persistences.ReadModels.Store;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IStoreQueryRepository
    {
        IQueryable<StoreReadModel> GetStoresListQueryable();
        IQueryable<StoreReadModel> GetStoreByIdQueryable(int roleId);
        IQueryable<StoreSelectReadModel> GetStoresSelectQueryable();
    }
}
