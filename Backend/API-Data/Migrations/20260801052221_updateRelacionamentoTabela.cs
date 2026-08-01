using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API_Data.Migrations
{
    /// <inheritdoc />
    public partial class updateRelacionamentoTabela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContaFixaTags_ContaFixa_ContaFixaId",
                table: "ContaFixaTags");

            migrationBuilder.RenameColumn(
                name: "ContaFixaId",
                table: "ContaFixaTags",
                newName: "ContasFixasId");

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Atribuicao", "Nome" },
                values: new object[,]
                {
                    { 1, 0, "Moradia" },
                    { 2, 0, "Transporte" },
                    { 3, 0, "Alimentação" },
                    { 4, 0, "Lazer" },
                    { 5, 0, "Educação" },
                    { 6, 0, "Salário" },
                    { 7, 0, "Investimentos" },
                    { 8, 0, "Outros" },
                    { 9, 0, "Vale-Refeição" },
                    { 10, 0, "Vale-Transporte" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ContaFixaTags_ContaFixa_ContasFixasId",
                table: "ContaFixaTags",
                column: "ContasFixasId",
                principalTable: "ContaFixa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContaFixaTags_ContaFixa_ContasFixasId",
                table: "ContaFixaTags");

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.RenameColumn(
                name: "ContasFixasId",
                table: "ContaFixaTags",
                newName: "ContaFixaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContaFixaTags_ContaFixa_ContaFixaId",
                table: "ContaFixaTags",
                column: "ContaFixaId",
                principalTable: "ContaFixa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
