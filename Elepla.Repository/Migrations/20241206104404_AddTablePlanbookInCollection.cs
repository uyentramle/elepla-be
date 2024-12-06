using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elepla.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddTablePlanbookInCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook");

            migrationBuilder.DropIndex(
                name: "IX_Planbook_CollectionId",
                table: "Planbook");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Planbook");

            migrationBuilder.CreateTable(
                name: "PlanbookInCollection",
                columns: table => new
                {
                    PlanbookInCollectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlanbookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CollectionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanbookInCollection", x => x.PlanbookInCollectionId);
                    table.ForeignKey(
                        name: "FK_PlanbookInCollection_PlanbookCollection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "PlanbookCollection",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanbookInCollection_Planbook_PlanbookId",
                        column: x => x.PlanbookId,
                        principalTable: "Planbook",
                        principalColumn: "PlanbookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookInCollection_CollectionId",
                table: "PlanbookInCollection",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanbookInCollection_PlanbookId",
                table: "PlanbookInCollection",
                column: "PlanbookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanbookInCollection");

            migrationBuilder.AddColumn<string>(
                name: "CollectionId",
                table: "Planbook",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Planbook_CollectionId",
                table: "Planbook",
                column: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Planbook_PlanbookCollection_CollectionId",
                table: "Planbook",
                column: "CollectionId",
                principalTable: "PlanbookCollection",
                principalColumn: "CollectionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
