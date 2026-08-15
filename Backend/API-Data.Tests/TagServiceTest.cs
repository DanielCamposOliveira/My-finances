using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Repository;
using API_Data.src.Services;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;

namespace API_Data.Tests
{
    public class TagServiceTest : TestBase
    {
        string id = Guid.NewGuid().ToString();
        string UserID = "21c8c222-6811-467e-8c1b-18f941349411";

        public TagServiceTest(ITestOutputHelper output) : base(output)
        {
            EscreverLinha("Registro: " + DateTime.Now);
        }

        /// <summary>
        /// Cria uma instância do serviço TagService com os repositórios necessários.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected TagService _TagService(AppDbContext dbContext)
        {
            var repo = new TagRepository (dbContext);

            return new TagService(repo);
        }


        [Fact]
        public async Task CriarTag()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _TagService(dbContext);
            
            // Criamos uma Tag Aleatora
            var nomeTagUnico = $"Tag_{Guid.NewGuid():N}";

            var dto = new CriarTagDto
            {
                Nome = nomeTagUnico
            };

   
            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.CriarTag(dto, UserID);

            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================

            Assert.NotNull(resultado); // verifica se o resultado não é Null

            // Converte o IResult para o tipo concreto Created (que é o que Results.Created() )
            var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created>(resultado);

            // Agora você consegue acessar o StatusCode com segurança!
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);

        }


       



        [Fact]
        public async Task ListaTags()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _TagService(dbContext);


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.ListaTags(UserID);


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================
            Assert.NotNull(resultado); // verifica se o resultado não é Null

            // Converte o IResult para o tipo concreto Created (que é o que Results.Created() )
            var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<TagResponseDto>>>(resultado);

            // Valida o StatusCode 200 OK
            Assert.Equal(StatusCodes.Status200OK, createdResult.StatusCode);

            List<TagResponseDto> tags = createdResult.Value;

            Assert.NotNull(tags); // verifica se não é null
            Assert.NotEmpty(tags); // verifica se não esta vazio


        }
    }
}
