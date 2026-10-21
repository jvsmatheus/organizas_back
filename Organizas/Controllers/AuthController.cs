using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organizas.Dtos.Request.User;

namespace Organizas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        //private readonly 
        public async Task<IActionResult> Register(RegisterUserRequestDto request, CancellationToken cancellationToken)
        {

            return Ok();
        }
    }
}
