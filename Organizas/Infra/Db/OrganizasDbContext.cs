using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Organizas.Entities;

namespace Organizas.Infra.Db
{
    public class OrganizasDbContext : IdentityDbContext<User>
    {
        public OrganizasDbContext(DbContextOptions<OrganizasDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShoppingListItem>()
                .Property(item => item.Quantity)
                .HasPrecision(12, 3);

            modelBuilder.Entity<ShoppingListItem>()
                .Property(item => item.EstimatedUnitPrice)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ShoppingList>()
                .ToTable("ShoppingLists")
                .HasMany(list => list.Items)
                .WithOne()
                .HasForeignKey(item => item.ShoppingListId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
        public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    }
}
