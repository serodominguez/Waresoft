namespace Application.Dtos.Response.Role
{
    public record RoleResponseDto
    {
        public int IdRole { get; init; }
        public string? RoleName { get; init; }
        public string? AuditCreateDate { get; init; }
        public string? StatusRole { get; init; }
    }
}
