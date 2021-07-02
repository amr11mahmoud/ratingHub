using Microsoft.EntityFrameworkCore.Migrations;

namespace Rating.Migrations
{
    public partial class AddProductCategoryTabel_AlterSomeAttr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCategId",
                table: "ProductTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MarketPlaceId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentCategId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplates_ProductCategId",
                table: "ProductTemplates",
                column: "ProductCategId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MarketPlaceId",
                table: "Products",
                column: "MarketPlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MarketPlaces_MarketPlaceId",
                table: "Products",
                column: "MarketPlaceId",
                principalTable: "MarketPlaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTemplates_ProductCategories_ProductCategId",
                table: "ProductTemplates",
                column: "ProductCategId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MarketPlaces_MarketPlaceId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTemplates_ProductCategories_ProductCategId",
                table: "ProductTemplates");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductTemplates_ProductCategId",
                table: "ProductTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Products_MarketPlaceId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductCategId",
                table: "ProductTemplates");

            migrationBuilder.DropColumn(
                name: "MarketPlaceId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Comments");
        }
    }
}
