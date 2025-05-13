using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Projekt.API.Migrations
{
    /// <inheritdoc />
    public partial class CleanMigrationver2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "ID",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
