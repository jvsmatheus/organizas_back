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

        public async Task<List<ShoppingListResponseDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.ShoppingLists
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(shoppingList => new ShoppingListResponseDto(
                    shoppingList.Id,
                    shoppingList.Name,
                    shoppingList.PurchaseDate
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<ShoppingListDetailResponseDto> GetAsync(int id, CancellationToken cancellationToken)
        {
            var shoppingList = await _context.ShoppingLists
                .AsNoTracking()
                .Where(list => list.Id == id)
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

        public async Task<ShoppingListResponseDto> CreateAsync(CreateShoppingListDto request, CancellationToken cancellationToken)
        {
            await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

            var shoppingList = new ShoppingList()
            {
                Name = request.Name.Trim(),
                PurchaseDate = request.PurchaseDate
            };

            await _context.ShoppingLists.AddAsync(shoppingList, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new ShoppingListResponseDto(
                shoppingList.Id,
                shoppingList.Name,
                shoppingList.PurchaseDate
            );
        }

        public async Task<ShoppingListResponseDto> UpdateAsync(int id, UpdateShoppingListDto request, CancellationToken cancellationToken)
        {
            await _updateValidator.ValidateAndThrowAsync(
                request,
                cancellationToken
            );

            var shoppingList = await _context.ShoppingLists.SingleOrDefaultAsync(item => item.Id == id, cancellationToken) ?? throw new ItemNotFoundException("Lista de compras não encontrada");

            shoppingList.Name = request.Name.Trim();
            shoppingList.PurchaseDate = request.PurchaseDate;

            await _context.SaveChangesAsync(cancellationToken);

            return new ShoppingListResponseDto(
                shoppingList.Id,
                shoppingList.Name,
                shoppingList.PurchaseDate
            );
        }

        public async Task<ShoppingListResponseDto> UpdatePurchaseDateAsync(int id, UpdateShoppingListPurchaseDateDto request, CancellationToken cancellationToken)
        {
            var shoppingList = await _context.ShoppingLists.SingleOrDefaultAsync(item => item.Id == id, cancellationToken) ?? throw new ItemNotFoundException("Lista de compras não encontrada");

            shoppingList.PurchaseDate = request.PurchaseDate;

            await _context.SaveChangesAsync(cancellationToken);

            return new ShoppingListResponseDto(
               shoppingList.Id,
               shoppingList.Name,
               shoppingList.PurchaseDate
            );
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var item = await _context.ShoppingLists.SingleOrDefaultAsync(item => item.Id == id, cancellationToken) ?? throw new ItemNotFoundException("Lista de compras não encontrada");

            _context.ShoppingLists.Remove(item);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
