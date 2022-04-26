using JWT.Token.Data;
using JWT.Token.Entities;
using JWT.Token.Models;
using JWT.Token.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JWT.Token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;
        public AccountController(IJwtService jwtService, ApplicationDbContext context)
        {
            _context = context;
            _jwtService = jwtService;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> AuthToken([FromBody] AuthRequest authRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponce { IsSuccess = false, Reason = "UserName and Password Must Be Provided" });
            }
            var authResponce = await _jwtService.GetTokenAsync(authRequest, HttpContext.Connection.RemoteIpAddress.ToString());
            if (authResponce == null)
                return Unauthorized();
            return Ok(authResponce);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponce { IsSuccess = false, Reason = "Token Must Be Provided" });
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var token = GetJWtToken(request.ExpiredToken);
            var userRefreshToken = _context.UserRefreshTokens.FirstOrDefault(x => x.IsInvalidated == false
            && x.Token == request.ExpiredToken
            && x.RefreshToken == request.RefreshToken
            && x.IpAddress == ipAddress);

            AuthResponce responce = ValidateDetails(token, userRefreshToken);
            if (!responce.IsSuccess)
                return BadRequest(responce);
            userRefreshToken.IsInvalidated = true;
            _context.UserRefreshTokens.Update(userRefreshToken);
            await _context.SaveChangesAsync();

            var userName = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId).Value;
            var authResponce = _jwtService.GetRefreshTokenAsync(ipAddress, userRefreshToken.UserID, userName);
            return Ok(authResponce);
        }

        private AuthResponce ValidateDetails(JwtSecurityToken token, UserRefreshToken userRefreshToken)
        {
            if (userRefreshToken == null)
                return new AuthResponce { IsSuccess = false, Reason = "Invalid Token Details" };
            if (token.ValidTo > DateTime.UtcNow)
                return new AuthResponce { IsSuccess = false, Reason = "Token Not Expired" };
            if (!userRefreshToken.IsActive)
                return new AuthResponce { IsSuccess = false, Reason = "Refresh Token Expired" };
            return new AuthResponce { IsSuccess = true };
        }

        private JwtSecurityToken GetJWtToken(string expiredToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(expiredToken);
        }
    }
}
