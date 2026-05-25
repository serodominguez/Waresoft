namespace Infrastructure.Persistences.ReadModels.User
{
    public record UserSelectReadModel
    {
        public int Id { get; init; }
        public string? Names { get; init; }
        public string? LastNames { get; init; }
        public bool? IsActive { get; init; }
    }
}
