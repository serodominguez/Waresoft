using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.User
{
    public class TokenRequestDto
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
