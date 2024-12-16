using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePlanbookShare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanBookShare_Planbook_PlanBookId",
                table: "PlanBookShare");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanBookShare_User_ShareBy",
                table: "PlanBookShare");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanBookShare",
                table: "PlanBookShare");

            migrationBuilder.DropColumn(
                name: "ShareTo",
                table: "PlanBookShare");

            migrationBuilder.DropColumn(
                name: "ShareToEmail",
                table: "PlanBookShare");

            migrationBuilder.DropColumn(
                name: "ShareType",
                table: "PlanBookShare");

            migrationBuilder.RenameTable(
                name: "PlanBookShare",
                newName: "PlanbookShare");

            migrationBuilder.RenameColumn(
                name: "PlanBookId",
                table: "PlanbookShare",
                newName: "PlanbookId");

            migrationBuilder.RenameColumn(
                name: "ShareBy",
                table: "PlanbookShare",
                newName: "SharedTo");

            migrationBuilder.RenameIndex(
                name: "IX_PlanBookShare_PlanBookId",
                table: "PlanbookShare",
                newName: "IX_PlanbookShare_PlanbookId");

            migrationBuilder.RenameIndex(
                name: "IX_PlanBookShare_ShareBy",
                table: "PlanbookShare",
                newName: "IX_PlanbookShare_SharedTo");

            migrationBuilder.AddColumn<string>(
                name: "SharedBy",
                table: "PlanbookShare",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PlanbookShare",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "PlanbookShare",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanbookShare",
                table: "PlanbookShare",
                column: "ShareId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookShare_SharedBy",
                table: "PlanbookShare",
                column: "SharedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookShare_UserId",
                table: "PlanbookShare",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookShare_UserId1",
                table: "PlanbookShare",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanbookShare_Planbook_PlanbookId",
                table: "PlanbookShare",
                column: "PlanbookId",
                principalTable: "Planbook",
                principalColumn: "PlanbookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanbookShare_User_SharedBy",
                table: "PlanbookShare",
                column: "SharedBy",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanbookShare_User_SharedTo",
                table: "PlanbookShare",
                column: "SharedTo",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanbookShare_User_UserId",
                table: "PlanbookShare",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanbookShare_User_UserId1",
                table: "PlanbookShare",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanbookShare_Planbook_PlanbookId",
                table: "PlanbookShare");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanbookShare_User_SharedBy",
                table: "PlanbookShare");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanbookShare_User_SharedTo",
                table: "PlanbookShare");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanbookShare_User_UserId",
                table: "PlanbookShare");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanbookShare_User_UserId1",
                table: "PlanbookShare");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanbookShare",
                table: "PlanbookShare");

            migrationBuilder.DropIndex(
                name: "IX_PlanbookShare_SharedBy",
                table: "PlanbookShare");

            migrationBuilder.DropIndex(
                name: "IX_PlanbookShare_UserId",
                table: "PlanbookShare");

            migrationBuilder.DropIndex(
                name: "IX_PlanbookShare_UserId1",
                table: "PlanbookShare");

            migrationBuilder.DropColumn(
                name: "SharedBy",
                table: "PlanbookShare");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlanbookShare");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "PlanbookShare");

            migrationBuilder.RenameTable(
                name: "PlanbookShare",
                newName: "PlanBookShare");

            migrationBuilder.RenameColumn(
                name: "PlanbookId",
                table: "PlanBookShare",
                newName: "PlanBookId");

            migrationBuilder.RenameColumn(
                name: "SharedTo",
                table: "PlanBookShare",
                newName: "ShareBy");

            migrationBuilder.RenameIndex(
                name: "IX_PlanbookShare_PlanbookId",
                table: "PlanBookShare",
                newName: "IX_PlanBookShare_PlanBookId");

            migrationBuilder.RenameIndex(
                name: "IX_PlanbookShare_SharedTo",
                table: "PlanBookShare",
                newName: "IX_PlanBookShare_ShareBy");

            migrationBuilder.AddColumn<string>(
                name: "ShareTo",
                table: "PlanBookShare",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShareToEmail",
                table: "PlanBookShare",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShareType",
                table: "PlanBookShare",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanBookShare",
                table: "PlanBookShare",
                column: "ShareId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanBookShare_Planbook_PlanBookId",
                table: "PlanBookShare",
                column: "PlanBookId",
                principalTable: "Planbook",
                principalColumn: "PlanbookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanBookShare_User_ShareBy",
                table: "PlanBookShare",
                column: "ShareBy",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
