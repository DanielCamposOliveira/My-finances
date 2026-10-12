using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cloud_API.Migrations
{
    /// <inheritdoc />
    public partial class addCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovieFormatBase",
                table: "StorageRegistry",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MovieFormatReady",
                table: "StorageRegistry",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieFormatBase",
                table: "StorageRegistry");

            migrationBuilder.DropColumn(
                name: "MovieFormatReady",
                table: "StorageRegistry");
        }
    }
}
