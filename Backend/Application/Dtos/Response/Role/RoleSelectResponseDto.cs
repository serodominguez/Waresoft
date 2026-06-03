namespace Application.Dtos.Response.Role
{
    public record RoleSelectResponseDto
    {
        public int IdRole { get; init; }
        public string? RoleName { get; init; }
    }
}
