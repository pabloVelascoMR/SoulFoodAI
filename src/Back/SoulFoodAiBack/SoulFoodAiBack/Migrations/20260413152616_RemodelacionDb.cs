using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulFoodAiBack.Migrations
{
    /// <inheritdoc />
    public partial class RemodelacionDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFoodPlansDaily_Meals_IdMeal",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFoodPlansDaily_Recipes_RecipeIdRecipe",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFoodPlansDaily_UserFoodPlansWeek_UserFoodPlanWeekIdUserFoodPlan",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropTable(
                name: "FoodPlanMeals");

            migrationBuilder.DropIndex(
                name: "IX_UserFoodPlansDaily_IdMeal",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropIndex(
                name: "IX_UserFoodPlansDaily_RecipeIdRecipe",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropIndex(
                name: "IX_UserFoodPlansDaily_UserFoodPlanWeekIdUserFoodPlan",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropColumn(
                name: "RecipeIdRecipe",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropColumn(
                name: "UserFoodPlanWeekIdUserFoodPlan",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropColumn(
                name: "IngredientsJson",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "TotalDailyKcal",
                table: "UserFoodPlansWeek",
                newName: "TotalWeeklyKcal");

            migrationBuilder.RenameColumn(
                name: "IdUserFoodPlan",
                table: "UserFoodPlansWeek",
                newName: "IdUserFoodPlanWeek");

            migrationBuilder.RenameColumn(
                name: "VegetablePercent",
                table: "UserFoodPlansDaily",
                newName: "TargetProtein");

            migrationBuilder.RenameColumn(
                name: "ProteinPercent",
                table: "UserFoodPlansDaily",
                newName: "TargetFat");

            migrationBuilder.RenameColumn(
                name: "IdMeal",
                table: "UserFoodPlansDaily",
                newName: "TargetKcal");

            migrationBuilder.RenameColumn(
                name: "FatPercent",
                table: "UserFoodPlansDaily",
                newName: "TargetCarbs");

            migrationBuilder.RenameColumn(
                name: "CarbPercent",
                table: "UserFoodPlansDaily",
                newName: "RealProtein");

            migrationBuilder.RenameColumn(
                name: "IdFoodPlanMeal",
                table: "UserFoodPlansDaily",
                newName: "IdUserFoodPlanDaily");

            migrationBuilder.AddColumn<int>(
                name: "IdGoal",
                table: "UserFoodPlansWeek",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdIntolerance",
                table: "UserFoodPlansWeek",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserFoodPlansWeek",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MealsPerDay",
                table: "UserFoodPlansWeek",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWeeklyCarbs",
                table: "UserFoodPlansWeek",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWeeklyFat",
                table: "UserFoodPlansWeek",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalWeeklyProtein",
                table: "UserFoodPlansWeek",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "IdUser",
                table: "UserFoodPlansDaily",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdUserFoodPlanWeek",
                table: "UserFoodPlansDaily",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserFoodPlansDaily",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "RealCarbs",
                table: "UserFoodPlansDaily",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RealFat",
                table: "UserFoodPlansDaily",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RealKcal",
                table: "UserFoodPlansDaily",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdMeal",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FoodPlanDailyRecipes",
                columns: table => new
                {
                    IdFoodPlanDailyRecipe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdUserFoodPlanDaily = table.Column<int>(type: "int", nullable: false),
                    IdRecipe = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPlanDailyRecipes", x => x.IdFoodPlanDailyRecipe);
                    table.ForeignKey(
                        name: "FK_FoodPlanDailyRecipes_Recipes_IdRecipe",
                        column: x => x.IdRecipe,
                        principalTable: "Recipes",
                        principalColumn: "IdRecipe",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodPlanDailyRecipes_UserFoodPlansDaily_IdUserFoodPlanDaily",
                        column: x => x.IdUserFoodPlanDaily,
                        principalTable: "UserFoodPlansDaily",
                        principalColumn: "IdUserFoodPlanDaily");
                    table.ForeignKey(
                        name: "FK_FoodPlanDailyRecipes_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser");
                });

            migrationBuilder.CreateTable(
                name: "RecipeUserIngredients",
                columns: table => new
                {
                    IdRecipeIngredient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdRecipe = table.Column<int>(type: "int", nullable: false),
                    IdIngredient = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeUserIngredients", x => x.IdRecipeIngredient);
                    table.ForeignKey(
                        name: "FK_RecipeUserIngredients_Ingredients_IdIngredient",
                        column: x => x.IdIngredient,
                        principalTable: "Ingredients",
                        principalColumn: "IdIngredient",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeUserIngredients_Recipes_IdRecipe",
                        column: x => x.IdRecipe,
                        principalTable: "Recipes",
                        principalColumn: "IdRecipe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansWeek_IdGoal",
                table: "UserFoodPlansWeek",
                column: "IdGoal");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansWeek_IdIntolerance",
                table: "UserFoodPlansWeek",
                column: "IdIntolerance");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansDaily_IdUser",
                table: "UserFoodPlansDaily",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_UserFoodPlansDaily_IdUserFoodPlanWeek",
                table: "UserFoodPlansDaily",
                column: "IdUserFoodPlanWeek");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_IdMeal",
                table: "Recipes",
                column: "IdMeal");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlanDailyRecipes_IdRecipe",
                table: "FoodPlanDailyRecipes",
                column: "IdRecipe");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlanDailyRecipes_IdUser",
                table: "FoodPlanDailyRecipes",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlanDailyRecipes_IdUserFoodPlanDaily",
                table: "FoodPlanDailyRecipes",
                column: "IdUserFoodPlanDaily");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeUserIngredients_IdIngredient",
                table: "RecipeUserIngredients",
                column: "IdIngredient");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeUserIngredients_IdRecipe",
                table: "RecipeUserIngredients",
                column: "IdRecipe");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Meals_IdMeal",
                table: "Recipes",
                column: "IdMeal",
                principalTable: "Meals",
                principalColumn: "IdMeal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFoodPlansDaily_UserFoodPlansWeek_IdUserFoodPlanWeek",
                table: "UserFoodPlansDaily",
                column: "IdUserFoodPlanWeek",
                principalTable: "UserFoodPlansWeek",
                principalColumn: "IdUserFoodPlanWeek");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFoodPlansDaily_Users_IdUser",
                table: "UserFoodPlansDaily",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFoodPlansWeek_Goals_IdGoal",
                table: "UserFoodPlansWeek",
                column: "IdGoal",
                principalTable: "Goals",
                principalColumn: "IdGoal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFoodPlansWeek_Intolerances_IdIntolerance",
                table: "UserFoodPlansWeek",
                column: "IdIntolerance",
                principalTable: "Intolerances",
                principalColumn: "IdIntolerance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Meals_IdMeal",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFoodPlansDaily_UserFoodPlansWeek_IdUserFoodPlanWeek",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFoodPlansDaily_Users_IdUser",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFoodPlansWeek_Goals_IdGoal",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFoodPlansWeek_Intolerances_IdIntolerance",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropTable(
                name: "FoodPlanDailyRecipes");

            migrationBuilder.DropTable(
                name: "RecipeUserIngredients");

            migrationBuilder.DropIndex(
                name: "IX_UserFoodPlansWeek_IdGoal",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropIndex(
                name: "IX_UserFoodPlansWeek_IdIntolerance",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropIndex(
                name: "IX_UserFoodPlansDaily_IdUser",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropIndex(
                name: "IX_UserFoodPlansDaily_IdUserFoodPlanWeek",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_IdMeal",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IdGoal",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropColumn(
                name: "IdIntolerance",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropColumn(
                name: "MealsPerDay",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropColumn(
                name: "TotalWeeklyCarbs",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropColumn(
                name: "TotalWeeklyFat",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropColumn(
                name: "TotalWeeklyProtein",
                table: "UserFoodPlansWeek");

            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropColumn(
                name: "IdUserFoodPlanWeek",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropColumn(
                name: "RealCarbs",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropColumn(
                name: "RealFat",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropColumn(
                name: "RealKcal",
                table: "UserFoodPlansDaily");

            migrationBuilder.DropColumn(
                name: "IdMeal",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "TotalWeeklyKcal",
                table: "UserFoodPlansWeek",
                newName: "TotalDailyKcal");

            migrationBuilder.RenameColumn(
                name: "IdUserFoodPlanWeek",
                table: "UserFoodPlansWeek",
                newName: "IdUserFoodPlan");

            migrationBuilder.RenameColumn(
                name: "TargetProtein",
                table: "UserFoodPlansDaily",
                newName: "VegetablePercent");

            migrationBuilder.RenameColumn(
                name: "TargetKcal",
                table: "UserFoodPlansDaily",
                newName: "IdMeal");

            migrationBuilder.RenameColumn(
                name: "TargetFat",
                table: "UserFoodPlansDaily",
                newName: "ProteinPercent");

            migrationBuilder.RenameColumn(
                name: "TargetCarbs",
                table: "UserFoodPlansDaily",
                newName: "FatPercent");

            migrationBuilder.RenameColumn(
                name: "RealProtein",
                table: "UserFoodPlansDaily",
                newName: "CarbPercent");

            migrationBuilder.RenameColumn(
                name: "IdUserFoodPlanDaily",
                table: "UserFoodPlansDaily",
                newName: "IdFoodPlanMeal");

            migrationBuilder.AddColumn<int>(
                name: "RecipeIdRecipe",
                table: "UserFoodPlansDaily",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserFoodPlanWeekIdUserFoodPlan",
                table: "UserFoodPlansDaily",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IngredientsJson",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FoodPlanMeals",
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
                    VegetableminPercent = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPlanMeals", x => x.IdFoodPlanMeal);
                    table.ForeignKey(
                        name: "FK_FoodPlanMeals_FoodPlans_IdFoodPlan",
                        column: x => x.IdFoodPlan,
                        principalTable: "FoodPlans",
                        principalColumn: "IdFoodPlan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodPlanMeals_Meals_IdMeal",
                        column: x => x.IdMeal,
                        principalTable: "Meals",
                        principalColumn: "IdMeal",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_FoodPlanMeals_IdFoodPlan",
                table: "FoodPlanMeals",
                column: "IdFoodPlan");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlanMeals_IdMeal",
                table: "FoodPlanMeals",
                column: "IdMeal");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFoodPlansDaily_Meals_IdMeal",
                table: "UserFoodPlansDaily",
                column: "IdMeal",
                principalTable: "Meals",
                principalColumn: "IdMeal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFoodPlansDaily_Recipes_RecipeIdRecipe",
                table: "UserFoodPlansDaily",
                column: "RecipeIdRecipe",
                principalTable: "Recipes",
                principalColumn: "IdRecipe");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFoodPlansDaily_UserFoodPlansWeek_UserFoodPlanWeekIdUserFoodPlan",
                table: "UserFoodPlansDaily",
                column: "UserFoodPlanWeekIdUserFoodPlan",
                principalTable: "UserFoodPlansWeek",
                principalColumn: "IdUserFoodPlan");
        }
    }
}
