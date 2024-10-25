using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesUpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_ServicePackage_PackageId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBank_Chapter_ChapterId",
                table: "QuestionBank");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPackage",
                table: "UserPackage");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserPackage");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ServicePackage");

            migrationBuilder.RenameColumn(
                name: "PackageId",
                table: "Payment",
                newName: "UserPackageId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_PackageId",
                table: "Payment",
                newName: "IX_Payment_UserPackageId");

            migrationBuilder.AddColumn<string>(
                name: "UserPackageId",
                table: "UserPackage",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ServicePackage",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ServicePackage",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "ChapterId",
                table: "QuestionBank",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsSaved",
                table: "PlanbookCollection",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TeachingTools",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherName",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SkillsObjective",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SchoolName",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "QualitiesObjective",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgeObjective",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "DurationInPeriods",
                table: "Planbook",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CollectionId",
                table: "Planbook",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ClassName",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Planbook",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPackage",
                table: "UserPackage",
                column: "UserPackageId");

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    AnswerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_Answer", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_Answer_QuestionBank_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBank",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanBookShare",
                columns: table => new
                {
                    ShareId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShareType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShareTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShareToEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanBookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShareBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_PlanBookShare", x => x.ShareId);
                    table.ForeignKey(
                        name: "FK_PlanBookShare_Planbook_PlanBookId",
                        column: x => x.PlanBookId,
                        principalTable: "Planbook",
                        principalColumn: "PlanbookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanBookShare_User_ShareBy",
                        column: x => x.ShareBy,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_QuestionId",
                table: "Answer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanBookShare_PlanBookId",
                table: "PlanBookShare",
                column: "PlanBookId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanBookShare_ShareBy",
                table: "PlanBookShare",
                column: "ShareBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_UserPackage_UserPackageId",
                table: "Payment",
                column: "UserPackageId",
                principalTable: "UserPackage",
                principalColumn: "UserPackageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook",
                column: "CollectionId",
                principalTable: "PlanbookCollection",
                principalColumn: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBank_Chapter_ChapterId",
                table: "QuestionBank",
                column: "ChapterId",
                principalTable: "Chapter",
                principalColumn: "ChapterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_UserPackage_UserPackageId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBank_Chapter_ChapterId",
                table: "QuestionBank");

            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "PlanBookShare");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPackage",
                table: "UserPackage");

            migrationBuilder.DropColumn(
                name: "UserPackageId",
                table: "UserPackage");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ServicePackage");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ServicePackage");

            migrationBuilder.DropColumn(
                name: "IsSaved",
                table: "PlanbookCollection");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Planbook");

            migrationBuilder.RenameColumn(
                name: "UserPackageId",
                table: "Payment",
                newName: "PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_UserPackageId",
                table: "Payment",
                newName: "IX_Payment_PackageId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserPackage",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "ServicePackage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ChapterId",
                table: "QuestionBank",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "TeachingTools",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TeacherName",
                table: "Planbook",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SkillsObjective",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SchoolName",
                table: "Planbook",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QualitiesObjective",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KnowledgeObjective",
                table: "Planbook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationInPeriods",
                table: "Planbook",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CollectionId",
                table: "Planbook",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClassName",
                table: "Planbook",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPackage",
                table: "UserPackage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_ServicePackage_PackageId",
                table: "Payment",
                column: "PackageId",
                principalTable: "ServicePackage",
                principalColumn: "PackageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook",
                column: "CollectionId",
                principalTable: "PlanbookCollection",
                principalColumn: "CollectionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBank_Chapter_ChapterId",
                table: "QuestionBank",
                column: "ChapterId",
                principalTable: "Chapter",
                principalColumn: "ChapterId");
        }
    }
}
