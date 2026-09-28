using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Organizas.Dtos;
using Organizas.Dtos.Request.ShoppingListItem;
using Organizas.Dtos.Response.ShoppingListItem;
using Organizas.Entities;
using Organizas.Infra.Db;
using Organizas.Services;

namespace Organizas.Controllers
{
    [Authorize]
    [ApiController]
    [Route("ShoppingList/{shoppingListId:int}/items")]
    public sealed class ShoppingListItemController : ControllerBase
    {
        private readonly ShoppingListItemService _shoppingListItemService;
        private readonly UserManager<User> _userManager;

        public ShoppingListItemController(
            ShoppingListItemService shoppingListItemService,
            UserManager<User> userManager
        ) {
            _shoppingListItemService = shoppingListItemService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromRoute] int shoppingListId, [FromBody] CreateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var item = await _shoppingListItemService.CreateAsync(userId, shoppingListId, request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, ApiResponseDto<ShoppingListItemResponseDto>.Ok(item, "Item criado com sucesso"));
        }

        [HttpPut("{itemId:int}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int shoppingListId, [FromRoute] int itemId, [FromBody] UpdateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var item = await _shoppingListItemService.UpdateAsync(userId, shoppingListId, itemId, request, cancellationToken);

            return Ok(ApiResponseDto<ShoppingListItemResponseDto>.Ok(item, "Item atualizado com sucesso"));

        }

        [HttpDelete("{itemId:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int shoppingListId, [FromRoute] int itemId, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _shoppingListItemService.DeleteAsync(userId, shoppingListId, itemId, cancellationToken);

            return Ok(ApiResponseDto<object>.Ok(null, "Item removido com sucesso"));

        }
    }
}
