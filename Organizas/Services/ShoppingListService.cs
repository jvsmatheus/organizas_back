using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Dtos.Response.ShoppingList;
using Organizas.Dtos.Response.ShoppingListItem;
using Organizas.Entities;
using Organizas.Exceptions;
using Organizas.Infra.Db;
using Organizas.Migrations;

namespace Organizas.Services
{
    public sealed class ShoppingListService
    {
        private readonly OrganizasDbContext _context;
        private readonly IValidator<CreateShoppingListDto> _createValidator;
        private readonly IValidator<UpdateShoppingListDto> _updateValidator;

        public ShoppingListService(
            OrganizasDbContext context,
            IValidator<CreateShoppingListDto> createValidator,
            IValidator<UpdateShoppingListDto> updateValidator
        ) {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<List<ShoppingListResponseDto>> GetAllAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.ShoppingLists
                .AsNoTracking()
                .Where(x => x.UserProfileId.Equals(userId))
                .OrderBy(x => x.Id)
                .Select(shoppingList => new ShoppingListResponseDto(
                    shoppingList.Id,
                    shoppingList.Name,
                    shoppingList.PurchaseDate
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<ShoppingListDetailResponseDto> GetAsync(string userId, int id, CancellationToken cancellationToken)
        {
            var shoppingList = await _context.ShoppingLists
                .AsNoTracking()
                .Where(list => list.Id == id && list.UserProfileId.Equals(userId))
                .Select(list => new ShoppingListDetailResponseDto(
                    list.Id,
                    list.Name,
                    list.PurchaseDate,
                    list.Items
                        .OrderBy(item => item.Id)
                        .Select(item => new ShoppingListItemResponseDto(
                            item.Id,
                            item.Name,
                            item.Quantity,
                            item.EstimatedUnitPrice,
                            item.Unit,
                            item.IsChecked
                        )).ToList()
                ))
                .SingleOrDefaultAsync(cancellationToken);

            return shoppingList ?? throw new ItemNotFoundException("Lista de compras não encontrada");
        }

        public async Task<ShoppingListResponseDto> CreateAsync(string userId, CreateShoppingListDto request, CancellationToken cancellationToken)
        {
            await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

            var shoppingList = new ShoppingList()
            {
                Name = request.Name.Trim(),
                PurchaseDate = request.PurchaseDate,
                UserProfileId = userId
            };

            await _context.ShoppingLists.AddAsync(shoppingList, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new ShoppingListResponseDto(
                shoppingList.Id,
                shoppingList.Name,
                shoppingList.PurchaseDate
            );
        }

        public async Task<ShoppingListResponseDto> UpdateAsync(string userId, int id, UpdateShoppingListDto request, CancellationToken cancellationToken)
        {
            await _updateValidator.ValidateAndThrowAsync(
                request,
                cancellationToken
            );

            var shoppingList = await _context.ShoppingLists
                .SingleOrDefaultAsync(
                    item => item.Id == id && item.UserProfileId.Equals(userId), cancellationToken
                ) ?? throw new ItemNotFoundException("Lista de compras não encontrada");

            shoppingList.Name = request.Name.Trim();

            if (request.PurchaseDate is not null && !request.PurchaseDate.Equals(shoppingList.PurchaseDate))
                shoppingList.PurchaseDate = request.PurchaseDate;

            await _context.SaveChangesAsync(cancellationToken);

            return new ShoppingListResponseDto(
                shoppingList.Id,
                shoppingList.Name,
                shoppingList.PurchaseDate
            );
        }

        public async Task DeleteAsync(string userId, int id, CancellationToken cancellationToken)
        {
            var item = await _context.ShoppingLists
                .SingleOrDefaultAsync(
                    item => item.Id == id && item.UserProfileId.Equals(userId), cancellationToken
                ) ?? throw new ItemNotFoundException("Lista de compras não encontrada");

            _context.ShoppingLists.Remove(item);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
