namespace Infrastructure.Persistences.ReadModels.Brand
{
    public record BrandSelectReadModel
    {
        public int Id { get; init; }
        public string? BrandName { get; init; }
        public bool IsActive { get; init; }
    }
}
