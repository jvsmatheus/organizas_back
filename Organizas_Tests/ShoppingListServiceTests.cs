using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Organizas.Entities;
using Organizas.Enum;
using Organizas.Exceptions;
using Organizas.Infra.Db;
using Organizas.Services;
using Organizas.Validations;
using Xunit;

namespace Organizas_Tests
{
    public class ShoppingListServiceTests
    {
        [Fact]
        public async Task MustReturnOnlyItemsFromRequestedList()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync();

            await using var context = await InstanceDbContext(connection);

            var shoppingListService = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator()
            );

            var itemChurrasco = new ShoppingListItem
            {
                Name = "Fraldinha",
                Quantity = 1,
                Unit = ShoppingListItemUnitEnum.Quilograma,
                IsChecked = true
            };

            var itemMercado = new ShoppingListItem
            {
                Name = "Arroz",
                Quantity = 1,
                Unit = ShoppingListItemUnitEnum.Quilograma,
                IsChecked = false
            };

            var listChurrasco = new ShoppingList
            {
                Name = "Churrasco",
                UserProfileId = "user-a",
                Items = [itemChurrasco]
            };

            var listMercado = new ShoppingList
            {
                Name = "Mercado",
                UserProfileId = "user-a",
                Items = [itemMercado]
            };

            context.ShoppingLists.AddRange(listChurrasco, listMercado);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            // Act
            var items = await shoppingListService.GetAsync("user-a", listChurrasco.Id, CancellationToken.None);

            // Assert
            var returnedItem = Assert.Single(items.Items);

            Assert.Equal(itemChurrasco.Id, returnedItem.Id);
        }

        [Fact]
        public async Task MustReturnEmptyCollectionWhenListHasNoItems()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync();

            await using var context = await InstanceDbContext(connection);

            var shoppingListService = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator()
            );

            var listMercado = new ShoppingList
            {
                Name = "Mercado",
                UserProfileId = "user-a",
                Items = []
            };

            await context.ShoppingLists.AddAsync(listMercado);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            // Act
            var items = await shoppingListService.GetAsync("user-a", listMercado.Id, CancellationToken.None);

            // Assert
            Assert.Empty(items.Items);
        }

        [Fact]
        public async Task MustThrowWhenShoppingListDoesNotExist()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync();

            await using var context = await InstanceDbContext(connection);

            var shoppingListService = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator()
            );

            // Act + Assert
            var ex = await Assert.ThrowsAsync<ItemNotFoundException>(() => shoppingListService.GetAsync("user-a", 1, CancellationToken.None));
        }

        private static async Task<OrganizasDbContext> InstanceDbContext(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<OrganizasDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new OrganizasDbContext(options);

            try
            {
                await context.Database.EnsureCreatedAsync();
                context.UserProfiles.Add(new UserProfile
                {
                    UserId = "user-a",
                    Name = "User A",
                    User = new User { Id = "user-a", UserName = "user-a" }
                });
                await context.SaveChangesAsync();
                return context;
            }
            catch
            {
                await context.DisposeAsync();
                throw;
            }
        }
    }
}
