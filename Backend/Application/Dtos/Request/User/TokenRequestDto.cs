using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.User
{
    public class TokenRequestDto
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
