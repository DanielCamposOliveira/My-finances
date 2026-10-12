using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cloud_API.Migrations
{
    /// <inheritdoc />
    public partial class addCamposNomes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileNameReady",
                table: "StorageRegistry",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileNameReady",
                table: "StorageRegistry");
        }
    }
}
