using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6)] // Minimum length for password
        public string Password { get; set; }
    }
}
