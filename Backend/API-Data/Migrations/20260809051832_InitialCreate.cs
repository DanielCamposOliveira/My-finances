using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API_Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Atribuicao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoricosFinanceiros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Mes = table.Column<int>(type: "integer", nullable: false),
                    TotalSaldo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    TotalDivida = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricosFinanceiros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

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
                    CategoriaId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_ContaFixa_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lancamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    QtdParcelas = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lancamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lancamentos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lancamentos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contafixa_parcela",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroParcela = table.Column<int>(type: "integer", nullable: false),
                    ValorParcela = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ContaFixaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contafixa_parcela", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contafixa_parcela_ContaFixa_ContaFixaId",
                        column: x => x.ContaFixaId,
                        principalTable: "ContaFixa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContaFixaTags",
                columns: table => new
                {
                    ContasFixasId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaFixaTags", x => new { x.ContasFixasId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_ContaFixaTags_ContaFixa_ContasFixasId",
                        column: x => x.ContasFixasId,
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

            migrationBuilder.CreateTable(
                name: "lancamento_parcela",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroParcela = table.Column<int>(type: "integer", nullable: false),
                    ValorParcela = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LancamentoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lancamento_parcela", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lancamento_parcela_Lancamentos_LancamentoId",
                        column: x => x.LancamentoId,
                        principalTable: "Lancamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LancamentoTags",
                columns: table => new
                {
                    LancamentosId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LancamentoTags", x => new { x.LancamentosId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_LancamentoTags_Lancamentos_LancamentosId",
                        column: x => x.LancamentosId,
                        principalTable: "Lancamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LancamentoTags_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Atribuicao", "Nome" },
                values: new object[,]
                {
                    { 1, 1, "Moradia" },
                    { 2, 1, "Transporte" },
                    { 3, 1, "Alimentação" },
                    { 4, 1, "Lazer" },
                    { 5, 1, "Educação" },
                    { 6, 2, "Salário" },
                    { 7, 1, "Investimentos" },
                    { 8, 1, "Outros" },
                    { 9, 2, "Vale-Refeição" },
                    { 10, 2, "Vale-Transporte" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Casa" },
                    { 2, "Carro" },
                    { 3, "Gastos" },
                    { 4, "Mercado" },
                    { 5, "Energia" },
                    { 6, "Agua" },
                    { 7, "Internet" },
                    { 8, "Outros" },
                    { 9, "Facudade" },
                    { 10, "Emprestimo" },
                    { 11, "Streaming" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContaFixa_CategoriaId",
                table: "ContaFixa",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContaFixa_UserId",
                table: "ContaFixa",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_contafixa_parcela_ContaFixaId",
                table: "contafixa_parcela",
                column: "ContaFixaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContaFixaParcela_Status_DataVencimento_ContaFixa",
                table: "contafixa_parcela",
                columns: new[] { "Status", "DataVencimento", "ContaFixaId" })
                .Annotation("Npgsql:IndexInclude", new[] { "ValorParcela" });

            migrationBuilder.CreateIndex(
                name: "IX_ContaFixaTags_TagsId",
                table: "ContaFixaTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosFinanceiros_Ano_Mes",
                table: "HistoricosFinanceiros",
                columns: new[] { "Ano", "Mes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lancamento_parcela_LancamentoId",
                table: "lancamento_parcela",
                column: "LancamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoParcela_Status_DataVencimento_Lancamento",
                table: "lancamento_parcela",
                columns: new[] { "Status", "DataVencimento", "LancamentoId" })
                .Annotation("Npgsql:IndexInclude", new[] { "ValorParcela" });

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_CategoriaId",
                table: "Lancamentos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_UserId",
                table: "Lancamentos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoTags_TagsId",
                table: "LancamentoTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contafixa_parcela");

            migrationBuilder.DropTable(
                name: "ContaFixaTags");

            migrationBuilder.DropTable(
                name: "HistoricosFinanceiros");

            migrationBuilder.DropTable(
                name: "lancamento_parcela");

            migrationBuilder.DropTable(
                name: "LancamentoTags");

            migrationBuilder.DropTable(
                name: "ContaFixa");

            migrationBuilder.DropTable(
                name: "Lancamentos");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
