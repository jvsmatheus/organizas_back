using Microsoft.AspNetCore.Mvc;
using Organizas.Dtos;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Dtos.Request.ShoppingListItem;
using Organizas.Entities;
using Organizas.Services;

namespace Organizas.Controllers
{
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
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var items = await _shoppingListService.GetAll(cancellationToken);

            return Ok(ApiResponseDto<List<ShoppingList>>.Ok(items, "Listagem de lista de compras feita com sucesso"));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var item = await _shoppingListService.Get(id, cancellationToken);

            return Ok(ApiResponseDto<ShoppingList>.Ok(item, "Lista de compras encontrada com sucesso"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShoppingListDto request, CancellationToken cancellationToken)
        {
            var item = await _shoppingListService.Create(request, cancellationToken);

            return StatusCode(201, ApiResponseDto<ShoppingList>.Ok(item, "Lista de compras criada com sucesso"));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShoppingListDto request, CancellationToken cancellationToken)
        {
            var item = await _shoppingListService.Update(id, request, cancellationToken);

            return Ok(ApiResponseDto<ShoppingList>.Ok(item, "Lista de compras atualizada com sucesso"));

        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _shoppingListService.Delete(id, cancellationToken);

            return StatusCode(204, ApiResponseDto<ShoppingList>.Ok(null, "Lista de compras removida com sucesso"));

        }
    }
}
