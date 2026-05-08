using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulFoodAiBack.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToRecipesAndUserMeasures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ChestMeasure",
                table: "UserDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HipMeasure",
                table: "UserDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LeftBicepMeasure",
                table: "UserDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LeftCuadricepsMeasure",
                table: "UserDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "LevelOfActivity",
                table: "UserDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "RightBicepMeasure",
                table: "UserDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RightCuadricepsMeasure",
                table: "UserDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WaistMeasure",
                table: "UserDatas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChestMeasure",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "HipMeasure",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "LeftBicepMeasure",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "LeftCuadricepsMeasure",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "LevelOfActivity",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "RightBicepMeasure",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "RightCuadricepsMeasure",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "WaistMeasure",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Recipes");
        }
    }
}
