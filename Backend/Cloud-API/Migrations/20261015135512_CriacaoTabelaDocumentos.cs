using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cloud_API.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoTabelaDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameOriginal = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NameArchived = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateAccessed = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NameArchived",
                table: "Documentos",
                column: "NameArchived");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documentos");
        }
    }
}
