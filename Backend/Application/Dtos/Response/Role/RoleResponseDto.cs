namespace Application.Dtos.Response.Role
{
    public class RoleResponseDto
    {
        public int IdRole { get; set; }
        public string? RoleName { get; set; }
        public string? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusRole { get; set; }
    }
}
