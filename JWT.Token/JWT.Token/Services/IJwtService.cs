using JWT.Token.Models;

namespace JWT.Token.Services
{
    public interface IJwtService
    {
        Task<AuthResponce> GetTokenAsync(AuthRequest authRequest,string ipAddress);
        Task<AuthResponce> GetRefreshTokenAsync(string ipAddress,int userId,string userName);
        Task<bool> IsTokenValid(string accessToken,string ipAddress);
    }
}
