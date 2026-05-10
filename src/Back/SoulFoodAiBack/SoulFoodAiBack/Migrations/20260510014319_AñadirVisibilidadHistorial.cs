using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulFoodAiBack.Migrations
{
    /// <inheritdoc />
    public partial class AñadirVisibilidadHistorial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisibleInHistory",
                table: "UserFoodPlansWeek",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisibleInHistory",
                table: "UserFoodPlansWeek");
        }
    }
}
