namespace Application.Dtos.Response.User
{
    public record UserResponseDto
    {
        public int IdUser { get; init; }
        public string? UserName { get; init; }
        public byte[]? PasswordHash { get; init; }
        public string? Names { get; init; }
        public string? LastNames { get; init; }
        public string? IdentificationNumber { get; init; }
        public string? PhoneNumber { get; init; }
        public int IdRole { get; init; }
        public string? RoleName { get; init; }
        public int IdStore { get; init; }
        public string? StoreName { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusUser { get; init; }
    }
}
