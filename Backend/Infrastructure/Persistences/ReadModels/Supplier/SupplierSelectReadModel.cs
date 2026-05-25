namespace Infrastructure.Persistences.ReadModels.Supplier
{
    public record SupplierSelectReadModel
    {
        public int Id { get; init; }
        public string? CompanyName { get; init; }
        public bool IsActive { get; init; }
    }
}
