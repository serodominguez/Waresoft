namespace Application.Dtos.Request.Permission
{
    public record PermissionRequestDto
    {
        public int IdPermission { get; init; }
        public bool Status { get; init; }
    }
}
