using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableCurriculumFramework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CurriculumFramework",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CurriculumFramework",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CurriculumFramework",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CurriculumFramework",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CurriculumFramework",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CurriculumFramework",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CurriculumFramework",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CurriculumFramework");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CurriculumFramework");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CurriculumFramework");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CurriculumFramework");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CurriculumFramework");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CurriculumFramework");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CurriculumFramework");
        }
    }
}
