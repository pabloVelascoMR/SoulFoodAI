using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulFoodAiBack.Migrations
{
    /// <inheritdoc />
    public partial class FixRecipesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUser",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RecipeDescription",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_IdUser",
                table: "Recipes",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Users_IdUser",
                table: "Recipes",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Users_IdUser",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_IdUser",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeDescription",
                table: "Recipes");
        }
    }
}
