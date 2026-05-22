namespace Application.Dtos.Response.User
{
    public record UserSelectResponseDto
    {
        public int IdUser { get; init; }
        public string? UserName { get; init; }
    }
}
