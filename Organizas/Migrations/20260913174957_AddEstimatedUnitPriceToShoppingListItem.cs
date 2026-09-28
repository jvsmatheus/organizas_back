using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organizas.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimatedUnitPriceToShoppingListItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedUnitPrice",
                table: "ShoppingListItems",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedUnitPrice",
                table: "ShoppingListItems");
        }
    }
}
