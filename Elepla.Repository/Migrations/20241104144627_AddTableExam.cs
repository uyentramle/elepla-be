using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddTableExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Planbook_Lesson_LessonId",
                table: "Planbook");

            migrationBuilder.DropForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "CurriculumFramework",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    ExamId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Time = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_Exam", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_Exam_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionInExam",
                columns: table => new
                {
                    QuestionInExamId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExamId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionInExam", x => x.QuestionInExamId);
                    table.ForeignKey(
                        name: "FK_QuestionInExam_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionInExam_QuestionBank_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuestionBank",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exam_UserId",
                table: "Exam",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionInExam_ExamId",
                table: "QuestionInExam",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionInExam_QuestionId",
                table: "QuestionInExam",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Planbook_Lesson_LessonId",
                table: "Planbook",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook",
                column: "CollectionId",
                principalTable: "PlanbookCollection",
                principalColumn: "CollectionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Planbook_Lesson_LessonId",
                table: "Planbook");

            migrationBuilder.DropForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook");

            migrationBuilder.DropTable(
                name: "QuestionInExam");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "CurriculumFramework");

            migrationBuilder.AddForeignKey(
                name: "FK_Planbook_Lesson_LessonId",
                table: "Planbook",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook",
                column: "CollectionId",
                principalTable: "PlanbookCollection",
                principalColumn: "CollectionId");
        }
    }
}
