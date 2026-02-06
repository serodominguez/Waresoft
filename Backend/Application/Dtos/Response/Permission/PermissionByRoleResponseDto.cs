namespace Application.Dtos.Response.Permission
{
    public class PermissionByRoleResponseDto
    {
        public int IdPermission { get; set; }
        public int IdRole { get; set; }
        public int IdModule { get; set; }
        public string? ModuleName { get; set; }
        public int IdAction { get; set; }
        public string? ActionName { get; set; }
        public bool Status { get; set; }
    }
}
