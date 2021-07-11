using Microsoft.EntityFrameworkCore.Migrations;

namespace Rating.Migrations
{
    public partial class AlterProductTableRemoveCommentsAndLocalReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LocalReviews",
                table: "Products");

            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "Products",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalReviews",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
