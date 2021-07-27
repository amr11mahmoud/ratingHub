using Microsoft.EntityFrameworkCore.Migrations;

namespace Rating.Migrations
{
    public partial class AddOnSiteReviewsInProductsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "ProductReviews");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductReviews",
                newName: "Review");

            migrationBuilder.AddColumn<float>(
                name: "Rate",
                table: "ProductReviews",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ProductReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId",
                table: "ProductReviews",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                table: "ProductReviews",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_ProductId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProductReviews");

            migrationBuilder.RenameColumn(
                name: "Review",
                table: "ProductReviews",
                newName: "Name");

            migrationBuilder.AddColumn<double>(
                name: "Value",
                table: "ProductReviews",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
