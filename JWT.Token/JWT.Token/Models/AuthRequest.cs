using System.ComponentModel.DataAnnotations;

namespace JWT.Token.Models
{
    public class AuthRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
