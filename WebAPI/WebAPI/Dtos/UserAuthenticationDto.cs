using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos
{
    public class UserAuthenticationDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
