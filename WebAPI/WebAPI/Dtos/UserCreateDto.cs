using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos
{
    public class UserCreateDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
