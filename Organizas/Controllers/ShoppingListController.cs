using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Organizas.Dtos;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Dtos.Response.ShoppingList;
using Organizas.Entities;
using Organizas.Services;

namespace Organizas.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public sealed class ShoppingListController : ControllerBase
    {
        private readonly ShoppingListService _shoppingListService;
        private readonly UserManager<User> _userManager;

        public ShoppingListController(
            ShoppingListService shoppingListService,
            UserManager<User> userManager
        )
        {
            _shoppingListService = shoppingListService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var items = await _shoppingListService.GetAllAsync(userId, cancellationToken);

            return Ok(ApiResponseDto<List<ShoppingListResponseDto>>.Ok(items, "Listagem de lista de compras feita com sucesso"));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var item = await _shoppingListService.GetAsync(userId, id, cancellationToken);

            return Ok(ApiResponseDto<ShoppingListDetailResponseDto>.Ok(item, "Lista de compras encontrada com sucesso"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateShoppingListDto request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var item = await _shoppingListService.CreateAsync(userId, request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, ApiResponseDto<ShoppingListResponseDto>.Ok(item, "Lista de compras criada com sucesso"));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateShoppingListDto request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var item = await _shoppingListService.UpdateAsync(userId, id, request, cancellationToken);

            return Ok(ApiResponseDto<ShoppingListResponseDto>.Ok(item, "Lista de compras atualizada com sucesso"));

        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _shoppingListService.DeleteAsync(userId, id, cancellationToken);

            return Ok(ApiResponseDto<object>.Ok(null, "Lista de compras removida com sucesso"));

        }
    }
}
