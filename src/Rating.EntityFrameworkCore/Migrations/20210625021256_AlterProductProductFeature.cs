using Microsoft.EntityFrameworkCore.Migrations;

namespace Rating.Migrations
{
    public partial class AlterProductProductFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductProductFeatures_ProductFeatures_FeatureId",
                table: "ProductProductFeatures");

            migrationBuilder.DropIndex(
                name: "IX_ProductProductFeatures_FeatureId",
                table: "ProductProductFeatures");

            migrationBuilder.AddColumn<int>(
                name: "ProductFeatureId",
                table: "ProductProductFeatures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductProductFeatures_ProductFeatureId",
                table: "ProductProductFeatures",
                column: "ProductFeatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductProductFeatures_ProductFeatures_ProductFeatureId",
                table: "ProductProductFeatures",
                column: "ProductFeatureId",
                principalTable: "ProductFeatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductProductFeatures_ProductFeatures_ProductFeatureId",
                table: "ProductProductFeatures");

            migrationBuilder.DropIndex(
                name: "IX_ProductProductFeatures_ProductFeatureId",
                table: "ProductProductFeatures");

            migrationBuilder.DropColumn(
                name: "ProductFeatureId",
                table: "ProductProductFeatures");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProductFeatures_FeatureId",
                table: "ProductProductFeatures",
                column: "FeatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductProductFeatures_ProductFeatures_FeatureId",
                table: "ProductProductFeatures",
                column: "FeatureId",
                principalTable: "ProductFeatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
