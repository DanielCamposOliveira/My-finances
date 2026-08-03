using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Indice_ContaFixaParcela_LancamentoParcela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LancamentoParcela_Status_DataVencimento_Lancamento",
                table: "lancamento_parcela",
                columns: new[] { "Status", "DataVencimento", "LancamentoId" })
                .Annotation("Npgsql:IndexInclude", new[] { "ValorParcela" });

            migrationBuilder.CreateIndex(
                name: "IX_ContaFixaParcela_Status_DataVencimento_ContaFixa",
                table: "contafixa_parcela",
                columns: new[] { "Status", "DataVencimento", "ContaFixaId" })
                .Annotation("Npgsql:IndexInclude", new[] { "ValorParcela" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LancamentoParcela_Status_DataVencimento_Lancamento",
                table: "lancamento_parcela");

            migrationBuilder.DropIndex(
                name: "IX_ContaFixaParcela_Status_DataVencimento_ContaFixa",
                table: "contafixa_parcela");
        }
    }
}
