using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Organizas.Entities;
using Organizas.Infra.Db;

namespace Organizas_Tests
{
    internal static class ShoppingIsolationTestData
    {
        public const string UserA = "user-a";
        public const string UserB = "user-b";
        public const int ListA = 10;
        public const int ListB = 20;
        public const int ItemA = 30;

        // Coincide com ListA para detectar autorização usando o ID do item.
        public const int ItemB = 10;

        public static async Task<OrganizasDbContext> InstanceDbContext(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<OrganizasDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new OrganizasDbContext(options);

            try
            {
                await context.Database.EnsureCreatedAsync();

                context.UserProfiles.AddRange(
                    new UserProfile
                    {
                        UserId = UserA,
                        Name = "User A",
                        User = new User { Id = UserA, UserName = UserA }
                    },
                    new UserProfile
                    {
                        UserId = UserB,
                        Name = "User B",
                        User = new User { Id = UserB, UserName = UserB }
                    });

                context.ShoppingLists.AddRange(
                    new ShoppingList
                    {
                        Id = ListA,
                        Name = "Lista A",
                        UserProfileId = UserA,
                        Items = [new ShoppingListItem
                        {
                            Id = ItemA, Name = "Arroz", Quantity = 1,
                            EstimatedUnitPrice = 5, IsChecked = false
                        }]
                    },
                    new ShoppingList
                    {
                        Id = ListB,
                        Name = "Lista B",
                        UserProfileId = UserB,
                        Items = [new ShoppingListItem
                        {
                            Id = ItemB, Name = "Feijão", Quantity = 2,
                            EstimatedUnitPrice = 8, IsChecked = false
                        }]
                    });

                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
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
