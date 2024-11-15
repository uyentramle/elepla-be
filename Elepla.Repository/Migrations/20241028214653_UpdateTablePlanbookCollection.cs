using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePlanbookCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsSaved",
                table: "PlanbookCollection",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IsSaved",
                table: "PlanbookCollection",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
