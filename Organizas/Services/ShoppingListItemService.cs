using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request.ShoppingListItem;
using Organizas.Entities;
using Organizas.Exceptions;
using Organizas.Infra.Db;

namespace Organizas.Services
{
    public sealed class ShoppingListItemService
    {
        private readonly OrganizasDbContext _context;
        private readonly IValidator<CreateShoppingListItemDto> _createValidator;
        private readonly IValidator<UpdateShoppingListItemDto> _updateValidator;

        public ShoppingListItemService(
            OrganizasDbContext context,
            IValidator<CreateShoppingListItemDto> createValidator,
            IValidator<UpdateShoppingListItemDto> updateValidator
        ) {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<List<ShoppingListItem>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.ShoppingListItems.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);
        }

        public async Task<ShoppingListItem> Get(int id, CancellationToken cancellationToken)
        {
            return await _context.ShoppingListItems.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken) ?? throw new ItemNotFoundException();
        }

        public async Task<ShoppingListItem> Create(CreateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            await _createValidator.ValidateAndThrowAsync(
                request,
                cancellationToken
            );

            var listExists = await _context.ShoppingLists.AnyAsync(x => x.Id == request.ShoppingListId, cancellationToken);

            if (!listExists)
                 throw new ItemNotFoundException("Lista de compras não encontrada");

            var item = new ShoppingListItem()
            {
                Name = request.Name.Trim(),
                Quantity = request.Quantity,
                Unit = request.Unit,
                IsChecked = false,
                ShoppingListId = request.ShoppingListId
            };

            _context.ShoppingListItems.Add(item);

            await _context.SaveChangesAsync(cancellationToken);

            return item;
        }

        public async Task<ShoppingListItem> Update(int id, UpdateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            await _updateValidator.ValidateAndThrowAsync(
                request,
                cancellationToken
            );

            var item = await _context.ShoppingListItems.SingleOrDefaultAsync(item => item.Id == id, cancellationToken) ?? throw new ItemNotFoundException();

            item.Name = request.Name.Trim();
            item.Quantity = request.Quantity;
            item.Unit = request.Unit;
            item.IsChecked = request.IsChecked;
            item.ShoppingListId = request.ShoppingListId;

            await _context.SaveChangesAsync(cancellationToken);

            return item;
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            var item = await _context.ShoppingListItems.SingleOrDefaultAsync(item => item.Id == id, cancellationToken) ?? throw new ItemNotFoundException();

            _context.ShoppingListItems.Remove(item);

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
