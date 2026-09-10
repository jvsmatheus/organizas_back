using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Entities;
using Organizas.Exceptions;
using Organizas.Infra.Db;

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

        public async Task<List<ShoppingList>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.ShoppingLists.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);
        }

        public async Task<ShoppingList> Get(int id, CancellationToken cancellationToken)
        {
            return await _context.ShoppingLists.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken) ?? throw new ItemNotFoundException();
        }

        public async Task<ShoppingList> Create(CreateShoppingListDto request, CancellationToken cancellationToken)
        {
            await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

            var item = new ShoppingList()
            {
                Name = request.Name.Trim()
            };

            await _context.ShoppingLists.AddAsync(item, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return item;
        }

        public async Task<ShoppingList> Update(int id, UpdateShoppingListDto request, CancellationToken cancellationToken)
        {
            await _updateValidator.ValidateAndThrowAsync(
                request,
                cancellationToken
            );

            var item = await _context.ShoppingLists.SingleOrDefaultAsync(item => item.Id == id, cancellationToken) ?? throw new ItemNotFoundException();

            item.Name = request.Name.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return item;
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            var item = await _context.ShoppingLists.SingleOrDefaultAsync(item => item.Id == id, cancellationToken) ?? throw new ItemNotFoundException();

            _context.ShoppingLists.Remove(item);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
