using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Data.Migrations
{
    /// <inheritdoc />
    public partial class TabelaProofPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProofPayment",
                table: "lancamento_parcela",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofPayment",
                table: "contafixa_parcela",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProofPayment",
                table: "lancamento_parcela");

            migrationBuilder.DropColumn(
                name: "ProofPayment",
                table: "contafixa_parcela");
        }
    }
}
