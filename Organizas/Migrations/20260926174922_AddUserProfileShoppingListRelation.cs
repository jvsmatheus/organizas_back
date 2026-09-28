using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organizas.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileShoppingListRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserProfileId",
                table: "ShoppingLists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_UserProfileId",
                table: "ShoppingLists",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingLists_UserProfiles_UserProfileId",
                table: "ShoppingLists",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingLists_UserProfiles_UserProfileId",
                table: "ShoppingLists");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingLists_UserProfileId",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "ShoppingLists");
        }
    }
}
