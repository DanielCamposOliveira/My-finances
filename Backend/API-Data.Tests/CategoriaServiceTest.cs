using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Enum;
using API_Data.src.Repository;
using API_Data.src.Services;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;

namespace API_Data.Tests
{
    public class CategoriaServiceTest : TestBase
    {
        public CategoriaServiceTest(ITestOutputHelper output) : base(output)
        {
            EscreverLinha("Registro: " + DateTime.Now);
        }

        /// <summary>
        /// Cria uma instância do serviço TagService com os repositórios necessários.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected CategoriaService _CategoriaService(AppDbContext dbContext)
        {
            var repo = new CategoriaRepository(dbContext);
            return new CategoriaService(repo);
        }

        [Fact]
        public async Task CriarCategoria()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _CategoriaService(dbContext);

            var Dados = new CriarCategoriaDto
            {
                Nome = $"CategoriaTest_{Guid.NewGuid():N}",
                Atribuicao = Atribuicao.Ganho,
                userId = "21c8c222-6811-467e-8c1b-18f941349411"
            };


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.CriarCategoria(Dados, "21c8c222-6811-467e-8c1b-18f941349411");


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================

            var created = resultado as Microsoft.AspNetCore.Http.HttpResults.Created;

            Assert.NotNull(created);
            Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        }

        [Fact]
        public async Task ListaCategoria()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _CategoriaService(dbContext);


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.ListaCategoria("21c8c222-6811-467e-8c1b-18f941349411");


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================
            // Converte o IResult para o tipo concreto Created (que é o que Results.Created() )
            var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<CategoriaResponseDto>>>(resultado);


            // Valida o StatusCode 200 OK
            Assert.Equal(StatusCodes.Status200OK, createdResult.StatusCode);

            Assert.NotEmpty(createdResult.Value); // verifica se não esta vazio
        }






    }
}
