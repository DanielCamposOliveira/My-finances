using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API_Data.Migrations
{
    /// <inheritdoc />
    public partial class Tabela_ContaFixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LancamentoId",
                table: "Parcelas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ContaFixaId",
                table: "Parcelas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContaFixa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ValorBase = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DiaVencimento = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaFixa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContaFixa_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContaFixaTags",
                columns: table => new
                {
                    ContaFixaId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaFixaTags", x => new { x.ContaFixaId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_ContaFixaTags_ContaFixa_ContaFixaId",
                        column: x => x.ContaFixaId,
                        principalTable: "ContaFixa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContaFixaTags_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parcelas_ContaFixaId",
                table: "Parcelas",
                column: "ContaFixaId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Parcela_OrigemUnica",
                table: "Parcelas",
                sql: "(\"LancamentoId\" IS NOT NULL AND \"ContaFixaId\" IS NULL) OR (\"LancamentoId\" IS NULL AND \"ContaFixaId\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ContaFixa_CategoriaId",
                table: "ContaFixa",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContaFixaTags_TagsId",
                table: "ContaFixaTags",
                column: "TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcelas_ContaFixa_ContaFixaId",
                table: "Parcelas",
                column: "ContaFixaId",
                principalTable: "ContaFixa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parcelas_ContaFixa_ContaFixaId",
                table: "Parcelas");

            migrationBuilder.DropTable(
                name: "ContaFixaTags");

            migrationBuilder.DropTable(
                name: "ContaFixa");

            migrationBuilder.DropIndex(
                name: "IX_Parcelas_ContaFixaId",
                table: "Parcelas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Parcela_OrigemUnica",
                table: "Parcelas");

            migrationBuilder.DropColumn(
                name: "ContaFixaId",
                table: "Parcelas");

            migrationBuilder.AlterColumn<int>(
                name: "LancamentoId",
                table: "Parcelas",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
