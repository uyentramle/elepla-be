using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableServicePackageAndPlanbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExportPdf",
                table: "ServicePackage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExportWord",
                table: "ServicePackage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseAI",
                table: "ServicePackage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseTemplate",
                table: "ServicePackage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Planbook",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExportPdf",
                table: "ServicePackage");

            migrationBuilder.DropColumn(
                name: "ExportWord",
                table: "ServicePackage");

            migrationBuilder.DropColumn(
                name: "UseAI",
                table: "ServicePackage");

            migrationBuilder.DropColumn(
                name: "UseTemplate",
                table: "ServicePackage");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Planbook");
        }
    }
}
