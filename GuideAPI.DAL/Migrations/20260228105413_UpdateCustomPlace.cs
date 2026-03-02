using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuideAPI.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomPlace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "UserPlaces",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "UserPlaces",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "UserPlaces");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "UserPlaces");
        }
    }
}
