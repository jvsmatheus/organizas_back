using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos;
using Organizas.Dtos.Request.ShoppingListItem;
using Organizas.Entities;
using Organizas.Infra.Db;
using Organizas.Services;

namespace Organizas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class ShoppingListItemController : ControllerBase
    {
        private readonly ShoppingListItemService _shoppingListItemService;

        public ShoppingListItemController(
            ShoppingListItemService shoppingListItemService
        ) {
            _shoppingListItemService = shoppingListItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var items = await _shoppingListItemService.GetAll(cancellationToken);

            return Ok(ApiResponseDto<List<ShoppingListItem>>.Ok(items, "Listagem de itens feita com sucesso"));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var item = await _shoppingListItemService.Get(id, cancellationToken);

            return Ok(ApiResponseDto<ShoppingListItem>.Ok(item, "Item encontrado com sucesso"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            var item = await _shoppingListItemService.Create(request, cancellationToken);

            return StatusCode(201, ApiResponseDto<ShoppingListItem>.Ok(item, "Item criado com sucesso"));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            var item = await _shoppingListItemService.Update(id, request, cancellationToken);

            return Ok(ApiResponseDto<ShoppingListItem>.Ok(item, "Item atualizado com sucesso"));

        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _shoppingListItemService.Delete(id, cancellationToken);

            return StatusCode(204, ApiResponseDto<ShoppingListItem>.Ok(null, "Item removido com sucesso"));

        }
    }
}
