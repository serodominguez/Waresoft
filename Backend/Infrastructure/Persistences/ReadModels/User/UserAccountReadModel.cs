namespace Infrastructure.Persistences.ReadModels.User
{
    public record UserAccountReadModel
    {
        public int Id { get; init; }
        public string? UserName { get; init; }
        public byte[]? PasswordHash { get; init; }
        public byte[]? PasswordSalt { get; init; }
        public int IdRole { get; init; }
        public int IdStore { get; init; }
        public string? RoleName { get; init; }
        public string? StoreName { get; init; }
        public string? Type { get; init; }
    }
}
