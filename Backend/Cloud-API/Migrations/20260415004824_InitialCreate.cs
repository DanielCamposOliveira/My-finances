using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cloud_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StorageRegistry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TokenVideoBase = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    DataArquivoBaseUpload = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ArquivoBaseDownload = table.Column<bool>(type: "INTEGER", nullable: true),
                    DataArquivoBaseDownload = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArquivoConvertidoUpload = table.Column<bool>(type: "INTEGER", nullable: true),
                    DateArquivoConvertidoUpload = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArquivoConvertidoDownload = table.Column<bool>(type: "INTEGER", nullable: true),
                    DataArquivoConvertidoDownload = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageRegistry", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorageRegistry");
        }
    }
}
