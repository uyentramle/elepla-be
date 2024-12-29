using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableQuestionBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "QuestionBank",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "QuestionBank");
        }
    }
}
