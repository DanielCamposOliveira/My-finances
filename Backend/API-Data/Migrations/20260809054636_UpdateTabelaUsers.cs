using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTabelaUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HistoricosFinanceiros_Ano_Mes",
                table: "HistoricosFinanceiros");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "HistoricosFinanceiros",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Categorias",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 6,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 7,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 8,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 9,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 10,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 2,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 3,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 4,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 5,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 6,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 7,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 8,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 9,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 10,
                column: "UserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 11,
                column: "UserId",
                value: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId_Nome",
                table: "Tags",
                columns: new[] { "UserId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosFinanceiros_UserId_Ano_Mes",
                table: "HistoricosFinanceiros",
                columns: new[] { "UserId", "Ano", "Mes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UserId_Nome",
                table: "Categorias",
                columns: new[] { "UserId", "Nome" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Users_UserId",
                table: "Categorias",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricosFinanceiros_Users_UserId",
                table: "HistoricosFinanceiros",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_UserId",
                table: "Tags",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Users_UserId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricosFinanceiros_Users_UserId",
                table: "HistoricosFinanceiros");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_UserId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UserId_Nome",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_HistoricosFinanceiros_UserId_Ano_Mes",
                table: "HistoricosFinanceiros");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UserId_Nome",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HistoricosFinanceiros");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Categorias");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosFinanceiros_Ano_Mes",
                table: "HistoricosFinanceiros",
                columns: new[] { "Ano", "Mes" },
                unique: true);
        }
    }
}
