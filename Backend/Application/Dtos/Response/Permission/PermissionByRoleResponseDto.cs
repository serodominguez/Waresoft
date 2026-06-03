namespace Application.Dtos.Response.Permission
{
    public record PermissionByRoleResponseDto
    {
        public int IdPermission { get; init; }
        public int IdRole { get; init; }
        public int IdModule { get; init; }
        public string? ModuleName { get; init; }
        public int IdAction { get; init; }
        public string? ActionName { get; init; }
        public bool Status { get; init; }
    }
}
