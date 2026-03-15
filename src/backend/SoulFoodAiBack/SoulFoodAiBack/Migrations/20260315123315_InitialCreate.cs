using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulFoodAiBack.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodPlans",
                columns: table => new
                {
                    IdFoodPlan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodPlanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProteinPercent = table.Column<double>(type: "float", nullable: false),
                    CarbPercent = table.Column<double>(type: "float", nullable: false),
                    FatPercent = table.Column<double>(type: "float", nullable: false),
                    VegetableminPercent = table.Column<double>(type: "float", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPlans", x => x.IdFoodPlan);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    IdGoal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.IdGoal);
                });

            migrationBuilder.CreateTable(
                name: "Intolerances",
                columns: table => new
                {
                    IdIntolerance = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntoleranceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intolerances", x => x.IdIntolerance);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    IdMeal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MealName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.IdMeal);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    IdRecipe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IngredientsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Protein = table.Column<double>(type: "float", nullable: false),
                    Carbs = table.Column<double>(type: "float", nullable: false),
                    Fat = table.Column<double>(type: "float", nullable: false),
                    TotalKcal = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.IdRecipe);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "FoodPlanMeals",
                columns: table => new
                {
                    IdFoodPlanMeal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProteinPercent = table.Column<double>(type: "float", nullable: false),
                    CarbPercent = table.Column<double>(type: "float", nullable: false),
                    FatPercent = table.Column<double>(type: "float", nullable: false),
                    VegetableminPercent = table.Column<double>(type: "float", nullable: false),
                    IdFoodPlan = table.Column<int>(type: "int", nullable: false),
                    IdMeal = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "UserDatas",
                columns: table => new
                {
                    IdUserData = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    MealsPerDay = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdFoodPlan = table.Column<int>(type: "int", nullable: false),
                    IdGoal = table.Column<int>(type: "int", nullable: false),
                    IdIntolerance = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDatas", x => x.IdUserData);
                    table.ForeignKey(
                        name: "FK_UserDatas_FoodPlans_IdFoodPlan",
                        column: x => x.IdFoodPlan,
                        principalTable: "FoodPlans",
                        principalColumn: "IdFoodPlan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDatas_Goals_IdGoal",
                        column: x => x.IdGoal,
                        principalTable: "Goals",
                        principalColumn: "IdGoal",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDatas_Intolerances_IdIntolerance",
                        column: x => x.IdIntolerance,
                        principalTable: "Intolerances",
                        principalColumn: "IdIntolerance",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDatas_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFoodPlans",
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
                name: "UserIngredients",
                columns: table => new
                {
                    IdUserIngredient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Protein = table.Column<double>(type: "float", nullable: false),
                    Carbs = table.Column<double>(type: "float", nullable: false),
                    Fat = table.Column<double>(type: "float", nullable: false),
                    Kcal = table.Column<double>(type: "float", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIngredients", x => x.IdUserIngredient);
                    table.ForeignKey(
                        name: "FK_UserIngredients_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDiaries",
                columns: table => new
                {
                    IdUserDiary = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HungerLevel = table.Column<int>(type: "int", nullable: false),
                    EnergyLevel = table.Column<int>(type: "int", nullable: false),
                    SleepQuality = table.Column<int>(type: "int", nullable: false),
                    DietAdherence = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUserFoodPlan = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDiaries", x => x.IdUserDiary);
                    table.ForeignKey(
                        name: "FK_UserDiaries_UserFoodPlans_IdUserFoodPlan",
                        column: x => x.IdUserFoodPlan,
                        principalTable: "UserFoodPlans",
                        principalColumn: "IdUserFoodPlan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFoodPlanMeals",
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
                    UserFoodPlanIdUserFoodPlan = table.Column<int>(type: "int", nullable: true)
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
                name: "IX_FoodPlanMeals_IdFoodPlan",
                table: "FoodPlanMeals",
                column: "IdFoodPlan");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlanMeals_IdMeal",
                table: "FoodPlanMeals",
                column: "IdMeal");

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_IdFoodPlan",
                table: "UserDatas",
                column: "IdFoodPlan");

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_IdGoal",
                table: "UserDatas",
                column: "IdGoal");

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_IdIntolerance",
                table: "UserDatas",
                column: "IdIntolerance");

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_IdUser",
                table: "UserDatas",
                column: "IdUser",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDiaries_IdUserFoodPlan",
                table: "UserDiaries",
                column: "IdUserFoodPlan");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredients_IdUser",
                table: "UserIngredients",
                column: "IdUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodPlanMeals");

            migrationBuilder.DropTable(
                name: "UserDatas");

            migrationBuilder.DropTable(
                name: "UserDiaries");

            migrationBuilder.DropTable(
                name: "UserFoodPlanMeals");

            migrationBuilder.DropTable(
                name: "UserIngredients");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "Intolerances");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "UserFoodPlans");

            migrationBuilder.DropTable(
                name: "FoodPlans");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
