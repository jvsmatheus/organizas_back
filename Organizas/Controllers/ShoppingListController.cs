using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizas.Dtos;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Dtos.Response.ShoppingList;
using Organizas.Services;

namespace Organizas.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public sealed class ShoppingListController : ControllerBase
    {
        private readonly ShoppingListService _shoppingListService;

        public ShoppingListController(
            ShoppingListService shoppingListService
        )
        {
            _shoppingListService = shoppingListService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var items = await _shoppingListService.GetAllAsync(cancellationToken);

            return Ok(ApiResponseDto<List<ShoppingListResponseDto>>.Ok(items, "Listagem de lista de compras feita com sucesso"));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
        {
            var item = await _shoppingListService.GetAsync(id, cancellationToken);

            return Ok(ApiResponseDto<ShoppingListDetailResponseDto>.Ok(item, "Lista de compras encontrada com sucesso"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateShoppingListDto request, CancellationToken cancellationToken)
        {
            var item = await _shoppingListService.CreateAsync(request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, ApiResponseDto<ShoppingListResponseDto>.Ok(item, "Lista de compras criada com sucesso"));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateShoppingListDto request, CancellationToken cancellationToken)
        {
            var item = await _shoppingListService.UpdateAsync(id, request, cancellationToken);

            return Ok(ApiResponseDto<ShoppingListResponseDto>.Ok(item, "Lista de compras atualizada com sucesso"));

        }

        [HttpPatch("{id:int}/purchase-date")]
        public async Task<IActionResult> UpdatePurchaseDateAsync(int id, [FromBody] UpdateShoppingListPurchaseDateDto request, CancellationToken cancellationToken)
        {
            var item = await _shoppingListService.UpdatePurchaseDateAsync(id, request, cancellationToken);

            return Ok(ApiResponseDto<ShoppingListResponseDto>.Ok(item, "Data de compra atualizada com sucesso"));

        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _shoppingListService.DeleteAsync(id, cancellationToken);

            return Ok(ApiResponseDto<object>.Ok(null, "Lista de compras removida com sucesso"));

        }
    }
}
