using Microsoft.AspNetCore.Authorization;
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

        public ShoppingListItemController(
            ShoppingListItemService shoppingListItemService
        ) {
            _shoppingListItemService = shoppingListItemService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromRoute] int shoppingListId, [FromBody] CreateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            var item = await _shoppingListItemService.CreateAsync(shoppingListId, request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, ApiResponseDto<ShoppingListItemResponseDto>.Ok(item, "Item criado com sucesso"));
        }

        [HttpPut("{itemId:int}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int shoppingListId, [FromRoute] int itemId, [FromBody] UpdateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            var item = await _shoppingListItemService.UpdateAsync(shoppingListId, itemId, request, cancellationToken);

            return Ok(ApiResponseDto<ShoppingListItemResponseDto>.Ok(item, "Item atualizado com sucesso"));

        }

        [HttpDelete("{itemId:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int shoppingListId, [FromRoute] int itemId, CancellationToken cancellationToken)
        {
            await _shoppingListItemService.DeleteAsync(shoppingListId, itemId, cancellationToken);

            return Ok(ApiResponseDto<object>.Ok(null, "Item removido com sucesso"));

        }
    }
}
