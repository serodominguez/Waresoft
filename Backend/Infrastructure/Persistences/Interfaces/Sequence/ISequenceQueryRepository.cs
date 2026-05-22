namespace Infrastructure.Persistences.Interfaces.Sequence
{
    public interface ISequenceQueryRepository
    {
        Task<string> ViewProductCodeAsync();
    }
}
