using Microsoft.AspNetCore.Mvc;
using Organizas.Dtos;
using Organizas.Dtos.Request.Auth;
using Organizas.Services;

namespace Organizas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto request, CancellationToken cancellationToken)
        {
            await _authService.Register(request, cancellationToken);

            return Ok(ApiResponseDto<object?>.Ok(null, "Usuário cadastrado com sucesso"));
        }
    }
}
