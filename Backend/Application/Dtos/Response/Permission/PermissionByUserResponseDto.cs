namespace Application.Dtos.Response.Permission
{
    public record PermissionByUserResponseDto
    {
        public string Module { get; init; } = null!;
        public string Action { get; init; } = null!;
    }
}
