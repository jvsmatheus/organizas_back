using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organizas.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseDateToShoppingList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "PurchaseDate",
                table: "ShoppingLists",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "ShoppingLists");
        }
    }
}
