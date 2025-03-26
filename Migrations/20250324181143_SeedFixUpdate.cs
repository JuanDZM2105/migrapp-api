using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace migrapp_api.Migrations
{
    /// <inheritdoc />
    public partial class SeedFixUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Users",
                newName: "Country");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhonePrefix",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhonePrefix",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "Users",
                newName: "Address");
        }
    }
}
