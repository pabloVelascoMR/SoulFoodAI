using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulFoodAiBack.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarModelosYRenombrarTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDiaries_UserFoodPlans_IdUserFoodPlan",
                table: "UserDiaries");

            migrationBuilder.DropTable(
                name: "UserFoodPlanMeals");

            migrationBuilder.DropTable(
                name: "UserFoodPlans");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Goals",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FoodPlans",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserFoodPlansWeek",
                columns: table => new
                {
                    IdUserFoodPlan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalDailyKcal = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdFoodPlan = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFoodPlansWeek", x => x.IdUserFoodPlan);
                    table.ForeignKey(
                        name: "FK_UserFoodPlansWeek_FoodPlans_IdFoodPlan",
                        column: x => x.IdFoodPlan,
                        principalTable: "FoodPlans",
                        principalColumn: "IdFoodPlan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFoodPlansWeek_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFoodPlansDaily",
                columns: table => new
                {
                    IdFoodPlanMeal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProteinPercent = table.Column<double>(type: "float", nullable: false),
                    CarbPercent = table.Column<double>(type: "float", nullable: false),
                    FatPercent = table.Column<double>(type: "float", nullable: false),
                    VegetablePercent = table.Column<double>(type: "float", nullable: false),
                    IdFoodPlan = table.Column<int>(type: "int", nullable: false),
                    IdMeal = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipeIdRecipe = table.Column<int>(type: "int", nullable: true),
                    UserFoodPlanWeekIdUserFoodPlan = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFoodPlansDaily", x => x.IdFoodPlanMeal);
                    table.ForeignKey(
                        name: "FK_UserFoodPlansDaily_FoodPlans_IdFoodPlan",
                        column: x => x.IdFoodPlan,
                        principalTable: "FoodPlans",
                        principalColumn: "IdFoodPlan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFoodPlansDaily_Meals_IdMeal",
                        column: x => x.IdMeal,
                        principalTable: "Meals",
                        principalColumn: "IdMeal",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFoodPlansDaily_Recipes_RecipeIdRecipe",
                        column: x => x.RecipeIdRecipe,
                        principalTable: "Recipes",
                        principalColumn: "IdRecipe");
                    table.ForeignKey(
                        name: "FK_UserFoodPlansDaily_UserFoodPlansWeek_UserFoodPlanWeekIdUserFoodPlan",
                        column: x => x.UserFoodPlanWeekIdUserFoodPlan,
                        principalTable: "UserFoodPlansWeek",
                        principalColumn: "IdUserFoodPlan");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansDaily_IdFoodPlan",
                table: "UserFoodPlansDaily",
                column: "IdFoodPlan");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansDaily_IdMeal",
                table: "UserFoodPlansDaily",
                column: "IdMeal");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansDaily_RecipeIdRecipe",
                table: "UserFoodPlansDaily",
                column: "RecipeIdRecipe");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansDaily_UserFoodPlanWeekIdUserFoodPlan",
                table: "UserFoodPlansDaily",
                column: "UserFoodPlanWeekIdUserFoodPlan");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansWeek_IdFoodPlan",
                table: "UserFoodPlansWeek",
                column: "IdFoodPlan");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansWeek_IdUser",
                table: "UserFoodPlansWeek",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiaries_UserFoodPlansWeek_IdUserFoodPlan",
                table: "UserDiaries",
                column: "IdUserFoodPlan",
                principalTable: "UserFoodPlansWeek",
                principalColumn: "IdUserFoodPlan",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDiaries_UserFoodPlansWeek_IdUserFoodPlan",
                table: "UserDiaries");

            migrationBuilder.DropTable(
                name: "UserFoodPlansDaily");

            migrationBuilder.DropTable(
                name: "UserFoodPlansWeek");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "FoodPlans");

            migrationBuilder.CreateTable(
                name: "UserFoodPlans",
                columns: table => new
                {
                    IdUserFoodPlan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFoodPlan = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDailyKcal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFoodPlans", x => x.IdUserFoodPlan);
                    table.ForeignKey(
                        name: "FK_UserFoodPlans_FoodPlans_IdFoodPlan",
                        column: x => x.IdFoodPlan,
                        principalTable: "FoodPlans",
                        principalColumn: "IdFoodPlan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFoodPlans_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFoodPlanMeals",
                columns: table => new
                {
                    IdFoodPlanMeal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFoodPlan = table.Column<int>(type: "int", nullable: false),
                    IdMeal = table.Column<int>(type: "int", nullable: false),
                    CarbPercent = table.Column<double>(type: "float", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FatPercent = table.Column<double>(type: "float", nullable: false),
                    ProteinPercent = table.Column<double>(type: "float", nullable: false),
                    RecipeIdRecipe = table.Column<int>(type: "int", nullable: true),
                    UserFoodPlanIdUserFoodPlan = table.Column<int>(type: "int", nullable: true),
                    VegetablePercent = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFoodPlanMeals", x => x.IdFoodPlanMeal);
                    table.ForeignKey(
                        name: "FK_UserFoodPlanMeals_FoodPlans_IdFoodPlan",
                        column: x => x.IdFoodPlan,
                        principalTable: "FoodPlans",
                        principalColumn: "IdFoodPlan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFoodPlanMeals_Meals_IdMeal",
                        column: x => x.IdMeal,
                        principalTable: "Meals",
                        principalColumn: "IdMeal",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFoodPlanMeals_Recipes_RecipeIdRecipe",
                        column: x => x.RecipeIdRecipe,
                        principalTable: "Recipes",
                        principalColumn: "IdRecipe");
                    table.ForeignKey(
                        name: "FK_UserFoodPlanMeals_UserFoodPlans_UserFoodPlanIdUserFoodPlan",
                        column: x => x.UserFoodPlanIdUserFoodPlan,
                        principalTable: "UserFoodPlans",
                        principalColumn: "IdUserFoodPlan");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlanMeals_IdFoodPlan",
                table: "UserFoodPlanMeals",
                column: "IdFoodPlan");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlanMeals_IdMeal",
                table: "UserFoodPlanMeals",
                column: "IdMeal");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlanMeals_RecipeIdRecipe",
                table: "UserFoodPlanMeals",
                column: "RecipeIdRecipe");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlanMeals_UserFoodPlanIdUserFoodPlan",
                table: "UserFoodPlanMeals",
                column: "UserFoodPlanIdUserFoodPlan");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlans_IdFoodPlan",
                table: "UserFoodPlans",
                column: "IdFoodPlan");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlans_IdUser",
                table: "UserFoodPlans",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDiaries_UserFoodPlans_IdUserFoodPlan",
                table: "UserDiaries",
                column: "IdUserFoodPlan",
                principalTable: "UserFoodPlans",
                principalColumn: "IdUserFoodPlan",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
