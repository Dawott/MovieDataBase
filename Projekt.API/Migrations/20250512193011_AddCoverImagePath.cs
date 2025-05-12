using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImagePath",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 1,
                column: "CoverImagePath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 2,
                column: "CoverImagePath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 3,
                column: "CoverImagePath",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImagePath",
                table: "Movies");
        }
    }
}
