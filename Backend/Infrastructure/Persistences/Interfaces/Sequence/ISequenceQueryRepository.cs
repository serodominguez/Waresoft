namespace Infrastructure.Persistences.Interfaces
{
    public interface ISequenceQueryRepository
    {
        Task<string> ViewProductCodeAsync();
    }
}
