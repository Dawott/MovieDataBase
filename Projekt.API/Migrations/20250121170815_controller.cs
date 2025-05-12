using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Projekt.API.Migrations
{
    /// <inheritdoc />
    public partial class Controller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Clients_ClientId",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_ClientId",
                table: "Movies");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Movies",
                newName: "ClientID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Movies",
                newName: "ID");

            migrationBuilder.CreateTable(
                name: "ClientMovie",
                columns: table => new
                {
                    ClientsID = table.Column<int>(type: "int", nullable: false),
                    MoviesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMovie", x => new { x.ClientsID, x.MoviesID });
                    table.ForeignKey(
                        name: "FK_ClientMovie_Clients_ClientsID",
                        column: x => x.ClientsID,
                        principalTable: "Clients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientMovie_Movies_MoviesID",
                        column: x => x.MoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ID", "Lastname", "Name" },
                values: new object[,]
                {
                    { 1, "Kowalski", "Jan" },
                    { 2, "Nowak", "Anna" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "ID", "ClientID", "Name", "Rating", "Type" },
                values: new object[,]
                {
                    { 1, 1, "Harry Potter", 7.0, "Fantasy" },
                    { 2, 2, "Skazany na Shawshank", 8.6999999999999993, "Dramat" },
                    { 3, 1, "Coco", 7.5, "Familijny" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientMovie_MoviesID",
                table: "ClientMovie",
                column: "MoviesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientMovie");

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.RenameColumn(
                name: "ClientID",
                table: "Movies",
                newName: "ClientId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Movies",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ClientId",
                table: "Movies",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Clients_ClientId",
                table: "Movies",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
