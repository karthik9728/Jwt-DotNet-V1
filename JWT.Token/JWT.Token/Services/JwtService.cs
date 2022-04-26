using JWT.Token.Data;
using JWT.Token.Entities;
using JWT.Token.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWT.Token.Services
{
    public class JwtService : IJwtService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public JwtService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponce> GetRefreshTokenAsync(string ipAddress, int userId, string userName)
        {
            var RefreshToken = GenerateRefreshToken();
            var accessToken = GenerateToken(userName);
            return await SaveTokenDetails(ipAddress,userId,accessToken,RefreshToken);
        }

        public async Task<AuthResponce> GetTokenAsync(AuthRequest authRequest, string ipAddress)
        {
            var user = _context.Users.FirstOrDefault(x => x.userName.Equals(authRequest.UserName)
            && x.password.Equals(authRequest.Password));
            if (user == null)
            {
                return await Task.FromResult<AuthResponce>(null);
            }

            string tokenString = GenerateToken(user.userName);
            string refreshToken = GenerateRefreshToken();
            return await SaveTokenDetails(ipAddress, user.userId, tokenString, refreshToken);
        }

        private async Task<AuthResponce> SaveTokenDetails(string ipAddress, int userId, string tokenString, string refreshToken)
        {
            var userRefreshToken = new UserRefreshToken
            {
                CreationTime = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMinutes(5),
                IpAddress = ipAddress,
                IsInvalidated = false,
                RefreshToken = refreshToken,
                Token = tokenString,
                UserID = userId
            };

            await _context.UserRefreshTokens.AddAsync(userRefreshToken);
            await _context.SaveChangesAsync();
            return new AuthResponce { Token = tokenString, RefreshToken = refreshToken ,IsSuccess=true};
        }

        private string GenerateRefreshToken()
        {
            var byteArray = new byte[64];
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                cryptoProvider.GetBytes(byteArray);
                return Convert.ToBase64String(byteArray);
            }
        }

        private string GenerateToken(string userName)
        {
            var jwtKey = _configuration.GetValue<string>("JwtSettings:Key");
            var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

            var tokenHandler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,userName)
                }),
                Expires = DateTime.UtcNow.AddSeconds(80),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(descriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public async Task<bool> IsTokenValid(string accessToken, string ipAddress)
        {
            var isValid = _context.UserRefreshTokens.FirstOrDefault(x=>x.Token==accessToken && x.IpAddress==ipAddress) != null; 
            return await Task.FromResult(isValid);
        }
    }
}