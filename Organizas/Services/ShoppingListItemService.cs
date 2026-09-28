using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request.ShoppingListItem;
using Organizas.Dtos.Response.ShoppingListItem;
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

        public async Task<ShoppingListItemResponseDto> CreateAsync(string userId, int shoppingListId, CreateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            await _createValidator.ValidateAndThrowAsync(
                request,
                cancellationToken
            );

            var listExists = await _context.ShoppingLists
                .AnyAsync(list => list.Id == shoppingListId && list.UserProfileId.Equals(userId), cancellationToken);

            if (!listExists)
                throw new ItemNotFoundException("Lista de compras não encontrada");

            var item = new ShoppingListItem()
            {
                Name = request.Name.Trim(),
                Quantity = request.Quantity,
                EstimatedUnitPrice = request.EstimatedUnitPrice,
                Unit = request.Unit,
                IsChecked = false,
                ShoppingListId = shoppingListId
            };

            _context.ShoppingListItems.Add(item);

            await _context.SaveChangesAsync(cancellationToken);

            return new ShoppingListItemResponseDto(
                item.Id,
                item.Name,
                item.Quantity,
                item.EstimatedUnitPrice,
                item.Unit,
                item.IsChecked
            );
        }

        public async Task<ShoppingListItemResponseDto> UpdateAsync(string userId, int shoppingListId, int id, UpdateShoppingListItemDto request, CancellationToken cancellationToken)
        {
            await _updateValidator.ValidateAndThrowAsync(
                request,
                cancellationToken
            );

            var listExist = await _context.ShoppingLists.AnyAsync(list => list.Id == shoppingListId && list.UserProfileId.Equals(userId), cancellationToken);

            if (!listExist)
                throw new ItemNotFoundException("Lista de compras não encontrada");

            var item = await _context.ShoppingListItems
                .SingleOrDefaultAsync(item => item.Id == id && item.ShoppingListId == shoppingListId, cancellationToken) ?? throw new ItemNotFoundException("Item não encontrado nesta lista");

            item.Name = request.Name.Trim();
            item.Quantity = request.Quantity;
            item.EstimatedUnitPrice = request.EstimatedUnitPrice;
            item.Unit = request.Unit;
            item.IsChecked = request.IsChecked;

            await _context.SaveChangesAsync(cancellationToken);

            return new ShoppingListItemResponseDto(
                item.Id,
                item.Name,
                item.Quantity,
                item.EstimatedUnitPrice,
                item.Unit,
                item.IsChecked
            );
        }

        public async Task DeleteAsync(string userId, int shoppingListId, int id, CancellationToken cancellationToken)
        {
            var listExist = await _context.ShoppingLists.AnyAsync(list => list.Id == shoppingListId && list.UserProfileId.Equals(userId), cancellationToken);

            if (!listExist)
                throw new ItemNotFoundException("Lista de compras não encontrada");

            var item = await _context.ShoppingListItems
                .SingleOrDefaultAsync(item => item.Id == id && item.ShoppingListId == shoppingListId, cancellationToken) ?? throw new ItemNotFoundException("Item não encontrado nesta lista.");

            _context.ShoppingListItems.Remove(item);

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
