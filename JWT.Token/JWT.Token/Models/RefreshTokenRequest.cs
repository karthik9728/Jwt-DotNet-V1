using System.ComponentModel.DataAnnotations;

namespace JWT.Token.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        public string ExpiredToken { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
