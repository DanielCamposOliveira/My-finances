using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHistoricoFinanceiroAnual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DespesasPagas",
                table: "HistoricosFinanceiros",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DividasAnteriores",
                table: "HistoricosFinanceiros",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DespesasPagas",
                table: "HistoricosFinanceiros");

            migrationBuilder.DropColumn(
                name: "DividasAnteriores",
                table: "HistoricosFinanceiros");
        }
    }
}
