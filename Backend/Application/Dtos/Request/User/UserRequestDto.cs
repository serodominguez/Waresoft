namespace Application.Dtos.Request.User
{
    public class UserRequestDto
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Names { get; set; }
        public string? LastNames { get; set; }
        public string? IdentificationNumber { get; set; }
        public int? PhoneNumber { get; set; }
        public int IdRole { get; set; }
        public int IdStore { get; set; }
        public bool? UpdatePassword { get; set; }
    }
}
