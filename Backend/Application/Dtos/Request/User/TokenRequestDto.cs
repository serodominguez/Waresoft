using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.User
{
    public record TokenRequestDto
    {
        public string? UserName { get; init; }
        public string? Password { get; init; }
    }
}
