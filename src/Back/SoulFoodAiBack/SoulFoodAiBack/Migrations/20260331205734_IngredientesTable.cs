using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulFoodAiBack.Migrations
{
    /// <inheritdoc />
    public partial class IngredientesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Carbs",
                table: "UserIngredients");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "UserIngredients");

            migrationBuilder.DropColumn(
                name: "Fat",
                table: "UserIngredients");

            migrationBuilder.DropColumn(
                name: "Kcal",
                table: "UserIngredients");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserIngredients");

            migrationBuilder.DropColumn(
                name: "Protein",
                table: "UserIngredients");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "UserIngredients");

            migrationBuilder.AddColumn<int>(
                name: "IdIngredient",
                table: "UserIngredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    IdIngredient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Protein = table.Column<double>(type: "float", nullable: false),
                    Carbs = table.Column<double>(type: "float", nullable: false),
                    Fat = table.Column<double>(type: "float", nullable: false),
                    Kcal = table.Column<double>(type: "float", nullable: false),
                    OpenFoodFactsId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.IdIngredient);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredients_IdIngredient",
                table: "UserIngredients",
                column: "IdIngredient");

            migrationBuilder.AddForeignKey(
                name: "FK_UserIngredients_Ingredients_IdIngredient",
                table: "UserIngredients",
                column: "IdIngredient",
                principalTable: "Ingredients",
                principalColumn: "IdIngredient",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserIngredients_Ingredients_IdIngredient",
                table: "UserIngredients");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_UserIngredients_IdIngredient",
                table: "UserIngredients");

            migrationBuilder.DropColumn(
                name: "IdIngredient",
                table: "UserIngredients");

            migrationBuilder.AddColumn<double>(
                name: "Carbs",
                table: "UserIngredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "UserIngredients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Fat",
                table: "UserIngredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Kcal",
                table: "UserIngredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserIngredients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Protein",
                table: "UserIngredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "UserIngredients",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
