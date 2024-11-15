using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_ServicePackage_PackageId",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPackage",
                table: "UserPackage");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserPackage");

            migrationBuilder.RenameColumn(
                name: "MaxLessonPlans",
                table: "ServicePackage",
                newName: "MaxPlanbooks");

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

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "QuestionInExam",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AddressText",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPackage",
                table: "UserPackage",
                column: "UserPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_UserPackage_UserPackageId",
                table: "Payment",
                column: "UserPackageId",
                principalTable: "UserPackage",
                principalColumn: "UserPackageId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_UserPackage_UserPackageId",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPackage",
                table: "UserPackage");

            migrationBuilder.DropColumn(
                name: "UserPackageId",
                table: "UserPackage");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "QuestionInExam");

            migrationBuilder.DropColumn(
                name: "AddressText",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Payment");

            migrationBuilder.RenameColumn(
                name: "MaxPlanbooks",
                table: "ServicePackage",
                newName: "MaxLessonPlans");

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
        }
    }
}
