namespace Infrastructure.Persistences.ReadModels.Role
{
    public record RoleReadModel
    {
        public int Id { get; init; }
        public string? RoleName { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public bool Status { get; init; }
    }
}
