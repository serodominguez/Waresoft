namespace Application.Dtos.Request.User
{
    public record UserRequestDto
    {
        public string? UserName { get; init; }
        public string? Password { get; init; }
        public string? Names { get; init; }
        public string? LastNames { get; init; }
        public string? IdentificationNumber { get; init; }
        public string? PhoneNumber { get; init; }
        public int IdRole { get; init; }
        public int IdStore { get; init; }
        public bool? UpdatePassword { get; init; }
    }
}
