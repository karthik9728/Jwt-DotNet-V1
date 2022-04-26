using System.ComponentModel.DataAnnotations;

namespace JWT.Token.Entities
{
    public class User
    {
        [Key]
        public int userId { get; set; } 
        [Required]
        public string userName { get; set; } = string.Empty;
        [Required]
        public string password { get; set; } = string.Empty ;
        public List<UserRefreshToken> UserRefreshTokens { get; set; }
    }
}
