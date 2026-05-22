namespace Infrastructure.Persistences.ReadModels.Action
{
    public record ActionReadModel
    {
        public int Id { get; init; }
        public string? ActionName { get; init; }
        public bool Status { get; init; }
    }
}
