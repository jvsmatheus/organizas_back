using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organizas.Dtos;
using Organizas.Dtos.Request.User;
using Organizas.Services;

namespace Organizas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(
            AuthService authService
        )
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto request, CancellationToken cancellationToken)
        {
            await _authService.RegisterAsync(request, cancellationToken);

            return Ok(ApiResponseDto<object>.Ok(null, "Conta registrada com sucesso."));
        }
    }
}
