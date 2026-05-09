using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoulFoodAiBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDiaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "UserDiaries",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "AiReportResponse",
                table: "UserDiaries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiReportResponse",
                table: "UserDiaries");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "UserDiaries",
                newName: "Notes");
        }
    }
}
