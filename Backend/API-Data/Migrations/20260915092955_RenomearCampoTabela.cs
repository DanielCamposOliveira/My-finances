using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Data.Migrations
{
    /// <inheritdoc />
    public partial class RenomearCampoTabela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalSaldo",
                table: "HistoricosFinanceiros",
                newName: "Receitas");

            migrationBuilder.RenameColumn(
                name: "TotalDivida",
                table: "HistoricosFinanceiros",
                newName: "Despesas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Receitas",
                table: "HistoricosFinanceiros",
                newName: "TotalSaldo");

            migrationBuilder.RenameColumn(
                name: "Despesas",
                table: "HistoricosFinanceiros",
                newName: "TotalDivida");
        }
    }
}
