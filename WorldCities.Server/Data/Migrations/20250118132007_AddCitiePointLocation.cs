using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace WorldCities.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCitiePointLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cities_Lat",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_Lon",
                table: "Cities");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Cities",
                type: "geography",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Cities");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Lat",
                table: "Cities",
                column: "Lat");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Lon",
                table: "Cities",
                column: "Lon");
        }
    }
}
