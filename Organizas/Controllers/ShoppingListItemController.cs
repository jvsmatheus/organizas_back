using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request;
using Organizas.Entities;
using Organizas.Entities.ApiResponse;
using Organizas.Infra.Db;
using Organizas.Services;

namespace Organizas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingListItemController : ControllerBase
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

            return Ok(ApiResponse<List<ShoppingListItem>>.Ok(items, "Listagem de item feita com sucesso"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            await _shoppingListItemService.Create(request, cancellationToken);

            return Ok(ApiResponse<object>.Ok(null, "Item criado com sucesso"));
        }
    }
}
