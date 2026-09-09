using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request;
using Organizas.Entities;
using Organizas.Infra.Db;

namespace Organizas.Services
{
    public sealed class ShoppingListItemService
    {
        private readonly OrganizasDbContext _context;
        private readonly IValidator<CreateShoppingListItemDto> _validator;

        public ShoppingListItemService(
            IValidator<CreateShoppingListItemDto> validator,
            OrganizasDbContext context
        ) {
            _validator = validator;
            _context = context;
        }

        public async Task<List<ShoppingListItem>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.ShoppingListItems.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);
        }

        public async Task Create(CreateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(
                request,
                cancellationToken
            );

            var item = new ShoppingListItem()
            {
                Name = request.Name.Trim(),
                Quantity = request.Quantity,
                Unit = request.Unit,
                IsChecked = false
            };

            _context.ShoppingListItems.Add(item);

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
