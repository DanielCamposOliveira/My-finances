using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cloud_API.Migrations
{
    /// <inheritdoc />
    public partial class AddCampoQtdAcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QtdAcess",
                table: "Documentos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QtdAcess",
                table: "Documentos");
        }
    }
}
