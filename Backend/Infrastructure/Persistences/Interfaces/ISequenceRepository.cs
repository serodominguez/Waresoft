namespace Infrastructure.Persistences.Interfaces
{
    public interface ISequenceRepository
    {
        Task<string> GenerateMovementsCodeAsync(string sequenceName, string prefix, int storeId);
        Task<string> GenerateTransferCodeAsync(string sequenceName, string prefix);
    }
}
