using Microsoft.EntityFrameworkCore.Migrations;

namespace Rating.Migrations
{
    public partial class AddUserDarkThemeAndLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DarkTheme",
                table: "AbpUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DarkTheme",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "AbpUsers");
        }
    }
}
