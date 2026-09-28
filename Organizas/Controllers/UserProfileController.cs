using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Organizas.Dtos;
using Organizas.Dtos.Request.UserProfile;
using Organizas.Dtos.Response.UserProfile;
using Organizas.Entities;
using Organizas.Exceptions;
using Organizas.Services;

namespace Organizas.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public sealed class UserProfileController : ControllerBase
    {
        private readonly UserProfileService _service;
        private readonly UserManager<User> _userManager;
        

        public UserProfileController(
            UserProfileService service,
            UserManager<User> userManager
        )
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<UserProfileResponseDto>>> GetAsync(CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var profile = await _service.GetByUserIdAsync(userId, cancellationToken);

            if (profile is null)
                throw new ItemNotFoundException("Perfil ainda não cadastrado");

            return Ok(ApiResponseDto<UserProfileResponseDto>.Ok(profile, "Perfil obtido com sucesso"));
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponseDto<UserProfileResponseDto>>> SaveAsync([FromBody] UpdateUserProfileDto dto, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var profile = await _service.SaveAsync(userId, dto, cancellationToken);

            return Ok(ApiResponseDto<UserProfileResponseDto>.Ok(profile, "Perfil salvo com sucesso"));
        }
    }
}
