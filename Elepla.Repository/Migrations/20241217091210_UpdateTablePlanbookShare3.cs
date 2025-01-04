using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePlanbookShare3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanbookShare");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanbookShare",
                columns: table => new
                {
                    ShareId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlanbookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SharedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SharedTo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_PlanbookShare_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_PlanbookShare_User_UserId1",
                        column: x => x.UserId1,
                        principalTable: "User",
                        principalColumn: "UserId");
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

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookShare_UserId",
                table: "PlanbookShare",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookShare_UserId1",
                table: "PlanbookShare",
                column: "UserId1");
        }
    }
}
