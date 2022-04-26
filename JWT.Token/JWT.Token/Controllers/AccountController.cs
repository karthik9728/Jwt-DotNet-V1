using JWT.Token.Models;
using JWT.Token.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT.Token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        public AccountController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> AuthToken([FromBody] AuthRequest authRequest)
        {
            var token = await _jwtService.GetTokenAsync(authRequest);
            if (token == null)
                return Unauthorized();
            return Ok(new AuthResponce { Token = token });
        }
    }
}
