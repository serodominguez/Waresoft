namespace Infrastructure.Persistences.ReadModels.Category
{
    public record CategorySelectReadModel
    {
        public int Id { get; init; }
        public string? CategoryName { get; init; }
        public bool? IsActive { get; init; }
    }
}
