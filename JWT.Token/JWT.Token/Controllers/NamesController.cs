using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT.Token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NamesController : ControllerBase
    {
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetNames()
        {
            var names = await Task.FromResult(new List<string> { "Adam", "Max" });
            return Ok(names);
        }
    }
}
