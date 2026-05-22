namespace Infrastructure.Persistences.ReadModels.Role
{
    public record RoleSelectReadModel
    {
        public int Id { get; init; }
        public string? RoleName { get; init; }
        public bool Status { get; init; }
    }
}
