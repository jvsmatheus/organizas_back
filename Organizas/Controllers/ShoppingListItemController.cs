using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request;
using Organizas.Entities;
using Organizas.Infra.Db;

namespace Organizas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingListItemController : ControllerBase
    {
        private readonly IValidator<CreateShoppingListItemDto> _validator;
        private readonly OrganizasDbContext _context;

        public ShoppingListItemController(
            IValidator<CreateShoppingListItemDto> validator,
            OrganizasDbContext context
        ) { 
            _validator = validator;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.ShoppingListItems.AsNoTracking().OrderBy(x => x.Id).ToListAsync();

            return StatusCode(StatusCodes.Status200OK, items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShoppingListItemDto request)
        {
            await _validator.ValidateAndThrowAsync(
                request,
                HttpContext.RequestAborted
            );

            var item = new ShoppingListItem() { 
                Name = request.Name.Trim(),
                Quantity = request.Quantity,
                Unit = request.Unit,
                IsChecked = false
            };

            _context.ShoppingListItems.Add(item);

            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, item);

        }
    }
}
