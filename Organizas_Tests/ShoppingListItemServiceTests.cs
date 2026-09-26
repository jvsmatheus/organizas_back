using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request.ShoppingListItem;
using Organizas.Exceptions;
using Organizas.Services;
using Organizas.Validations;
using Xunit;
using static Organizas_Tests.ShoppingIsolationTestData;

namespace Organizas_Tests
{
    public class ShoppingListItemServiceTests
    {
        [Fact]
        public async Task MustRejectCreatingItemInAnotherUsersList()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListItemService(
                context,
                new CreateShoppingListItemValidator(),
                new UpdateShoppingListItemValidator());

            var request = new CreateShoppingListItemDto("Novo item", 1, 3, null);

            // Act + Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(
                () => service.CreateAsync(UserA, ListB, request, CancellationToken.None));

            context.ChangeTracker.Clear();
            Assert.Equal(new[] { ItemB, ItemA },
                await context.ShoppingListItems.OrderBy(item => item.Id).Select(item => item.Id).ToArrayAsync());
        }

        [Fact]
        public async Task MustCreateItemInOwnList()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListItemService(
                context,
                new CreateShoppingListItemValidator(),
                new UpdateShoppingListItemValidator());

            var request = new CreateShoppingListItemDto("Novo item", 1, 3, null);

            // Act
            var response = await service.CreateAsync(UserA, ListA, request, CancellationToken.None);

            // Assert
            context.ChangeTracker.Clear();
            var saved = await context.ShoppingListItems.SingleAsync(item => item.Id == response.Id);
            Assert.Equal(ListA, saved.ShoppingListId);
            Assert.Equal("Novo item", saved.Name);
            Assert.Single(await context.ShoppingListItems.Where(item => item.ShoppingListId == ListB).ToListAsync());
        }

        [Fact]
        public async Task MustRejectUpdatingAnotherUsersItem()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListItemService(
                context,
                new CreateShoppingListItemValidator(),
                new UpdateShoppingListItemValidator());

            // Act + Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(
                () => service.UpdateAsync(UserA, ListB, ItemB, new UpdateShoppingListItemDto("Alterado", 9, 99, null, true), CancellationToken.None));

            context.ChangeTracker.Clear();
            var saved = await context.ShoppingListItems.SingleAsync(item => item.Id == ItemB);
            Assert.Equal("Feijão", saved.Name);
            Assert.Equal(2m, saved.Quantity);
            Assert.Equal(8m, saved.EstimatedUnitPrice);
            Assert.Null(saved.Unit);
            Assert.False(saved.IsChecked);
            Assert.Equal(ListB, saved.ShoppingListId);
            Assert.Equal(2, await context.ShoppingListItems.CountAsync());
        }

        [Fact]
        public async Task MustRejectUpdatingItemOutsideRequestedList()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListItemService(
                context,
                new CreateShoppingListItemValidator(),
                new UpdateShoppingListItemValidator());

            // Act + Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(
                () => service.UpdateAsync(UserA, ListA, ItemB, new UpdateShoppingListItemDto("Alterado", 9, 99, null, true), CancellationToken.None));

            context.ChangeTracker.Clear();
            var saved = await context.ShoppingListItems.SingleAsync(item => item.Id == ItemB);
            Assert.Equal("Feijão", saved.Name);
            Assert.Equal(2m, saved.Quantity);
            Assert.Equal(8m, saved.EstimatedUnitPrice);
            Assert.Null(saved.Unit);
            Assert.False(saved.IsChecked);
            Assert.Equal(ListB, saved.ShoppingListId);
            Assert.Equal(2, await context.ShoppingListItems.CountAsync());
        }

        [Fact]
        public async Task MustRejectDeletingAnotherUsersItem()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListItemService(
                context,
                new CreateShoppingListItemValidator(),
                new UpdateShoppingListItemValidator());

            // Act + Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(
                () => service.DeleteAsync(UserA, ListB, ItemB, CancellationToken.None));

            context.ChangeTracker.Clear();
            var saved = await context.ShoppingListItems.SingleAsync(item => item.Id == ItemB);
            Assert.Equal("Feijão", saved.Name);
            Assert.Equal(2m, saved.Quantity);
            Assert.Equal(8m, saved.EstimatedUnitPrice);
            Assert.Null(saved.Unit);
            Assert.False(saved.IsChecked);
            Assert.Equal(ListB, saved.ShoppingListId);
            Assert.Equal(2, await context.ShoppingListItems.CountAsync());
        }

        [Fact]
        public async Task MustRejectDeletingItemOutsideRequestedList()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListItemService(
                context,
                new CreateShoppingListItemValidator(),
                new UpdateShoppingListItemValidator());

            // Act + Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(
                () => service.DeleteAsync(UserA, ListA, ItemB, CancellationToken.None));

            context.ChangeTracker.Clear();
            var saved = await context.ShoppingListItems.SingleAsync(item => item.Id == ItemB);
            Assert.Equal("Feijão", saved.Name);
            Assert.Equal(2m, saved.Quantity);
            Assert.Equal(8m, saved.EstimatedUnitPrice);
            Assert.Null(saved.Unit);
            Assert.False(saved.IsChecked);
            Assert.Equal(ListB, saved.ShoppingListId);
            Assert.Equal(2, await context.ShoppingListItems.CountAsync());
        }

        [Fact]
        public async Task MustUpdateOwnItemWithoutChangingAnotherUsersItem()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListItemService(
                context,
                new CreateShoppingListItemValidator(),
                new UpdateShoppingListItemValidator());

            var request = new UpdateShoppingListItemDto("Arroz integral", 3, 12, null, true);

            // Act
            await service.UpdateAsync(UserA, ListA, ItemA, request, CancellationToken.None);

            // Assert
            context.ChangeTracker.Clear();
            var saved = await context.ShoppingListItems.SingleAsync(item => item.Id == ItemA);
            Assert.Equal(request.Name, saved.Name);
            Assert.Equal(request.Quantity, saved.Quantity);
            Assert.Equal(request.EstimatedUnitPrice, saved.EstimatedUnitPrice);
            Assert.True(saved.IsChecked);
            Assert.Equal(ListA, saved.ShoppingListId);
            var other = await context.ShoppingListItems.SingleAsync(item => item.Id == ItemB);
            Assert.Equal("Feijão", other.Name);
            Assert.False(other.IsChecked);
        }

        [Fact]
        public async Task MustDeleteOwnItemWithoutDeletingAnotherUsersItem()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListItemService(
                context,
                new CreateShoppingListItemValidator(),
                new UpdateShoppingListItemValidator());

            // Act
            await service.DeleteAsync(UserA, ListA, ItemA, CancellationToken.None);

            // Assert
            context.ChangeTracker.Clear();
            Assert.Equal(ItemB, Assert.Single(await context.ShoppingListItems.ToListAsync()).Id);
            Assert.Equal(2, await context.ShoppingLists.CountAsync());
        }
    }
}
