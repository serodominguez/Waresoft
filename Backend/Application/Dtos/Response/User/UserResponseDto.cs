namespace Application.Dtos.Response.User
{
    public class UserResponseDto
    {
        public int IdUser { get; set; }
        public string? UserName { get; set; }
        public byte[]? PasswordHash { get; set; }
        public string? Names { get; set; }
        public string? LastNames { get; set; }
        public string? IdentificationNumber { get; set; }
        public int? PhoneNumber { get; set; }
        public int IdRole { get; set; }
        public string? RoleName { get; set; }
        public int IdStore { get; set; }
        public string? StoreName { get; set; }
        public string? AuditCreateDate { get; set; }
        public bool Status { get; set; }
        public string? StatusUser { get; set; }
    }
}
