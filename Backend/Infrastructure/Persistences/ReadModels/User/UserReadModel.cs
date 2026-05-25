namespace Infrastructure.Persistences.ReadModels.User
{
    public record UserReadModel
    {
        public int Id { get; init; }
        public string? UserName { get; init; }
        public string? Names { get; init; }
        public string? LastNames { get; init; }
        public string? IdentificationNumber { get; init; }
        public string? PhoneNumber { get; init; }
        public byte[]? PasswordHash { get; init; }
        public int IdRole { get; init; }
        public int IdStore { get; init; }
        public bool IsActive { get; init; }
        public DateTime? AuditCreateDate { get; init; }
        public string? RoleName { get; init; }
        public string? StoreName { get; init; }
    }
}
