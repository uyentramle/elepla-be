using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePlanbookShare2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanbookShare_User_UserId",
                table: "PlanbookShare");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanbookShare_User_UserId1",
                table: "PlanbookShare");

            migrationBuilder.DropIndex(
                name: "IX_PlanbookShare_UserId",
                table: "PlanbookShare");

            migrationBuilder.DropIndex(
                name: "IX_PlanbookShare_UserId1",
                table: "PlanbookShare");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlanbookShare");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "PlanbookShare");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
