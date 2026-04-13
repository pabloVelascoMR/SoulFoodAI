using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulFoodAiBack.Migrations
{
    /// <inheritdoc />
    public partial class TablasMultiplesIntolerancias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatas_Intolerances_IdIntolerance",
                table: "UserDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFoodPlansWeek_Intolerances_IdIntolerance",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropIndex(
                name: "IX_UserFoodPlansWeek_IdIntolerance",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropIndex(
                name: "IX_UserDatas_IdIntolerance",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "IdIntolerance",
                table: "UserFoodPlansWeek");

            migrationBuilder.RenameColumn(
                name: "IdRecipeIngredient",
                table: "RecipeUserIngredients",
                newName: "IdRecipeUserIngredient");

            migrationBuilder.CreateTable(
                name: "UserFoodPlanWeekIntolerances",
                columns: table => new
                {
                    IdUserFoodPlanWeekIntolerance = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUserFoodPlanWeek = table.Column<int>(type: "int", nullable: false),
                    IdIntolerance = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFoodPlanWeekIntolerances", x => x.IdUserFoodPlanWeekIntolerance);
                    table.ForeignKey(
                        name: "FK_UserFoodPlanWeekIntolerances_Intolerances_IdIntolerance",
                        column: x => x.IdIntolerance,
                        principalTable: "Intolerances",
                        principalColumn: "IdIntolerance",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFoodPlanWeekIntolerances_UserFoodPlansWeek_IdUserFoodPlanWeek",
                        column: x => x.IdUserFoodPlanWeek,
                        principalTable: "UserFoodPlansWeek",
                        principalColumn: "IdUserFoodPlanWeek",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserIntolerances",
                columns: table => new
                {
                    IdUserIntolerance = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdIntolerance = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserDataIdUserData = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIntolerances", x => x.IdUserIntolerance);
                    table.ForeignKey(
                        name: "FK_UserIntolerances_Intolerances_IdIntolerance",
                        column: x => x.IdIntolerance,
                        principalTable: "Intolerances",
                        principalColumn: "IdIntolerance",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserIntolerances_UserDatas_UserDataIdUserData",
                        column: x => x.UserDataIdUserData,
                        principalTable: "UserDatas",
                        principalColumn: "IdUserData");
                    table.ForeignKey(
                        name: "FK_UserIntolerances_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlanWeekIntolerances_IdIntolerance",
                table: "UserFoodPlanWeekIntolerances",
                column: "IdIntolerance");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlanWeekIntolerances_IdUserFoodPlanWeek",
                table: "UserFoodPlanWeekIntolerances",
                column: "IdUserFoodPlanWeek");

            migrationBuilder.CreateIndex(
                name: "IX_UserIntolerances_IdIntolerance",
                table: "UserIntolerances",
                column: "IdIntolerance");

            migrationBuilder.CreateIndex(
                name: "IX_UserIntolerances_IdUser",
                table: "UserIntolerances",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_UserIntolerances_UserDataIdUserData",
                table: "UserIntolerances",
                column: "UserDataIdUserData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFoodPlanWeekIntolerances");

            migrationBuilder.DropTable(
                name: "UserIntolerances");

            migrationBuilder.RenameColumn(
                name: "IdRecipeUserIngredient",
                table: "RecipeUserIngredients",
                newName: "IdRecipeIngredient");

            migrationBuilder.AddColumn<int>(
                name: "IdIntolerance",
                table: "UserFoodPlansWeek",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansWeek_IdIntolerance",
                table: "UserFoodPlansWeek",
                column: "IdIntolerance");

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_IdIntolerance",
                table: "UserDatas",
                column: "IdIntolerance");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDatas_Intolerances_IdIntolerance",
                table: "UserDatas",
                column: "IdIntolerance",
                principalTable: "Intolerances",
                principalColumn: "IdIntolerance",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFoodPlansWeek_Intolerances_IdIntolerance",
                table: "UserFoodPlansWeek",
                column: "IdIntolerance",
                principalTable: "Intolerances",
                principalColumn: "IdIntolerance");
        }
    }
}
