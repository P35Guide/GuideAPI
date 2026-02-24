using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuideAPI.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeletedConectingWithAcounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPlaces_AppUsers_AppUserId",
                table: "UserPlaces");

            migrationBuilder.DropIndex(
                name: "IX_UserPlaces_AppUserId",
                table: "UserPlaces");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "UserPlaces");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "UserPlaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserPlaces_AppUserId",
                table: "UserPlaces",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlaces_AppUsers_AppUserId",
                table: "UserPlaces",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
