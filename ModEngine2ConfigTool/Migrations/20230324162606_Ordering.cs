using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModEngine2ConfigTool.Migrations
{
    /// <inheritdoc />
    public partial class Ordering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DllsOrder",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModsOrder",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DllsOrder",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ModsOrder",
                table: "Profiles");
        }
    }
}
