using JWT.Token.Models;

namespace JWT.Token.Services
{
    public interface IJwtService
    {
        Task<string> GetTokenAsync(AuthRequest authRequest);
    }
}
