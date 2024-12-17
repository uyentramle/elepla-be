using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePlanbookShare4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanbookShare",
                columns: table => new
                {
                    ShareId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    PlanbookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SharedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SharedTo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanbookShare", x => x.ShareId);
                    table.ForeignKey(
                        name: "FK_PlanbookShare_Planbook_PlanbookId",
                        column: x => x.PlanbookId,
                        principalTable: "Planbook",
                        principalColumn: "PlanbookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanbookShare_User_SharedBy",
                        column: x => x.SharedBy,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanbookShare_User_SharedTo",
                        column: x => x.SharedTo,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookShare_PlanbookId",
                table: "PlanbookShare",
                column: "PlanbookId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookShare_SharedBy",
                table: "PlanbookShare",
                column: "SharedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookShare_SharedTo",
                table: "PlanbookShare",
                column: "SharedTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanbookShare");
        }
    }
}
