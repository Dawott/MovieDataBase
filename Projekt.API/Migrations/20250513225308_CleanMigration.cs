using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Projekt.API.Migrations
{
    /// <inheritdoc />
    public partial class CleanMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CoverImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Clients_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    MovieID = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ratings_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Movies_MovieID",
                        column: x => x.MovieID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    MovieID = table.Column<int>(type: "int", nullable: false),
                    RentalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rentals_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rentals_Movies_MovieID",
                        column: x => x.MovieID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "ID", "CoverImagePath", "IsAvailable", "Name", "Rating", "Type" },
                values: new object[,]
                {
                    { 1, null, true, "Harry Potter", 7.0, "Fantasy" },
                    { 2, null, true, "Skazany na Shawshank", 8.6999999999999993, "Dramat" },
                    { 3, null, true, "Coco", 7.5, "Familijny" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "CreatedAt", "Email", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jan.kowalski@example.com", "6rHxyLo/hht170O4wmTrsQ==.L1ZHuJGOmRef671i6CSWTUQ2wYz9ZltYpbeiiqO0rNA=", 0 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "anna.nowak@example.com", "ipo/aVenxn+dLqGuOgu2yQ==.L+ASz444Zsti+63zgaZ9l+y7gAYwJZcUKvOdsxSqBLg=", 0 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@example.com", "1TWd66Rq1k8EG4IHpVUeZQ==.2n+AK1JE5Nm5eG00sQsh7rdEzINf+hMZqDowexfEY4g=", 1 }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ID", "Lastname", "Name", "UserID" },
                values: new object[,]
                {
                    { 1, "Kowalski", "Jan", 1 },
                    { 2, "Nowak", "Anna", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientMovie_MoviesID",
                table: "ClientMovie",
                column: "MoviesID");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserID",
                table: "Clients",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ClientID_MovieID",
                table: "Ratings",
                columns: new[] { "ClientID", "MovieID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_MovieID",
                table: "Ratings",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_ClientID",
                table: "Rentals",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_MovieID",
                table: "Rentals",
                column: "MovieID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientMovie");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
