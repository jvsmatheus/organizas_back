using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Exceptions;
using Organizas.Services;
using Organizas.Validations;
using Xunit;
using static Organizas_Tests.ShoppingIsolationTestData;

namespace Organizas_Tests
{
    public class ShoppingListIsolationTests
    {
        [Fact]
        public async Task MustReturnOnlyListsFromRequestedUser()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator());

            // Act
            var lists = await service.GetAllAsync(UserA, CancellationToken.None);

            // Assert
            Assert.Equal(ListA, Assert.Single(lists).Id);
        }

        [Fact]
        public async Task MustRejectReadingAnotherUsersList()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator());

            // Act + Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(
                () => service.GetAsync(UserA, ListB, CancellationToken.None));
        }

        [Fact]
        public async Task MustCreateListForRequestedUser()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator());

            var request = new CreateShoppingListDto("Nova lista", null);

            // Act
            var response = await service.CreateAsync(UserA, request, CancellationToken.None);

            // Assert
            context.ChangeTracker.Clear();
            var saved = await context.ShoppingLists.SingleAsync(list => list.Id == response.Id);
            Assert.Equal(UserA, saved.UserProfileId);
            Assert.Equal("Nova lista", saved.Name);
        }

        [Fact]
        public async Task MustRejectUpdatingAnotherUsersList()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator());

            // Act + Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(
                () => service.UpdateAsync(UserA, ListB, new UpdateShoppingListDto("Alterada", new DateOnly(2026, 10, 1)), CancellationToken.None));

            context.ChangeTracker.Clear();
            var saved = await context.ShoppingLists.SingleAsync(list => list.Id == ListB);
            Assert.Equal("Lista B", saved.Name);
            Assert.Equal(UserB, saved.UserProfileId);
            Assert.Null(saved.PurchaseDate);
            Assert.Equal(2, await context.ShoppingLists.CountAsync());
            Assert.Equal(2, await context.ShoppingListItems.CountAsync());
        }

        [Fact]
        public async Task MustRejectDeletingAnotherUsersList()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator());

            // Act + Assert
            await Assert.ThrowsAsync<ItemNotFoundException>(
                () => service.DeleteAsync(UserA, ListB, CancellationToken.None));

            context.ChangeTracker.Clear();
            var saved = await context.ShoppingLists.SingleAsync(list => list.Id == ListB);
            Assert.Equal("Lista B", saved.Name);
            Assert.Equal(UserB, saved.UserProfileId);
            Assert.Null(saved.PurchaseDate);
            Assert.Equal(2, await context.ShoppingLists.CountAsync());
            Assert.Equal(2, await context.ShoppingListItems.CountAsync());
        }

        [Fact]
        public async Task MustUpdateOwnListWithoutChangingAnotherUsersList()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator());

            var request = new UpdateShoppingListDto("Lista atualizada", new DateOnly(2026, 10, 1));

            // Act
            await service.UpdateAsync(UserA, ListA, request, CancellationToken.None);

            // Assert
            context.ChangeTracker.Clear();
            var ownList = await context.ShoppingLists.SingleAsync(list => list.Id == ListA);
            var otherList = await context.ShoppingLists.SingleAsync(list => list.Id == ListB);
            Assert.Equal("Lista atualizada", ownList.Name);
            Assert.Equal(request.PurchaseDate, ownList.PurchaseDate);
            Assert.Equal("Lista B", otherList.Name);
        }

        [Fact]
        public async Task MustDeleteOwnListAndItemsWithoutDeletingAnotherUsersData()
        {
            // Arrange
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            await using var context = await ShoppingIsolationTestData.InstanceDbContext(connection);

            var service = new ShoppingListService(
                context,
                new CreateShoppingListValidator(),
                new UpdateShoppingListValidator());

            // Act
            await service.DeleteAsync(UserA, ListA, CancellationToken.None);

            // Assert
            context.ChangeTracker.Clear();
            Assert.Equal(ListB, Assert.Single(await context.ShoppingLists.ToListAsync()).Id);
            Assert.Equal(ItemB, Assert.Single(await context.ShoppingListItems.ToListAsync()).Id);
        }
    }
}
