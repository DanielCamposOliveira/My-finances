using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.DTOs.ContasFixas;
using API_Data.src.Enum;
using API_Data.src.Model;
using API_Data.src.Repository;
using API_Data.src.Services;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;

namespace API_Data.Tests
{
    public class ContasFixasServiceTest : TestBase
    {
        string id = Guid.NewGuid().ToString();
        string UserID = "21c8c222-6811-467e-8c1b-18f941349411";

        public ContasFixasServiceTest(ITestOutputHelper output) : base(output)
        {
            EscreverLinha("Registro: " + DateTime.Now);
        }

        /// <summary>
        /// Cria uma instância do serviço TagService com os repositórios necessários.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected ContasFixasService _ContasFixasService(AppDbContext dbContext)
        {
            var repo = new ContasFixasRepository(dbContext);

            return new ContasFixasService(repo);
        }



        [Fact]
        public async Task CriarContaFixaAsync()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ContasFixasService(dbContext);

            var dados = new Create
            {
                CategoriaId = 34,
                Descricao = $"ContaFixaTest_{Guid.NewGuid():N}",
                DiaVencimento = 5,
                ValorBase = 155,
                TagIds = []
            };

            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.CriarContaFixaAsync(dados, UserID);


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
        public async Task GerarFaturasMesAsync()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ContasFixasService(dbContext);


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.GerarFaturasMesAsync(UserID);


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================


            Assert.True(
                resultado is Microsoft.AspNetCore.Http.HttpResults.Ok ok && ok.StatusCode == StatusCodes.Status200OK
                ||
                resultado is Microsoft.AspNetCore.Http.HttpResults.Created created && created.StatusCode == StatusCodes.Status201Created,
                "O resultado deve ser OK (200) ou Created (201)."
                );

        }

        [Fact]
        public async Task ListaTodasContasFixa()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ContasFixasService(dbContext);


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.ListaTodasContasFixa(UserID);


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //============================================================
            // Converte o IResult para o tipo concreto Created (que é o que Results.Created() )
            var ValorResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<ContaFixaResponse>>>(resultado);

            // Valida o StatusCode 200 OK
            Assert.Equal(StatusCodes.Status200OK, ValorResult.StatusCode);

            List<ContaFixaResponse> ListaConta = ValorResult.Value;
            Assert.NotNull(ListaConta); // verifica se não é null
            Assert.NotEmpty(ListaConta); // verifica se não esta vazio
        }

        [Fact]
        public async Task ListFaturaPendenteAsync()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ContasFixasService(dbContext);


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.ListFaturaPendenteAsync(UserID);


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //============================================================
            // Converte o IResult para o tipo concreto Created (que é o que Results.Created() )
            var ValorResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<ParcelasResponse>>>(resultado);

            // Valida o StatusCode 200 OK
            Assert.Equal(StatusCodes.Status200OK, ValorResult.StatusCode);

            List<ParcelasResponse> ListaConta = ValorResult.Value;

            Assert.NotNull(ListaConta); // verifica se não é null
           
        }


        [Fact]
        public async Task UpdateStatusParcela()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ContasFixasService(dbContext);

            ParcelaUpdateStatus dto = new ParcelaUpdateStatus
            {
             ParcelaId = 1,
             Status = StatusParcela.Aberto
            };


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.UpdateStatusParcela(dto, UserID);


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //============================================================
            // Converte o IResult para o tipo concreto Created (que é o que Results.Created() )
            var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created>(resultado);

            // Agora você consegue acessar o StatusCode com segurança!
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }



        [Fact]
        public async Task UpdateValorParcela()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ContasFixasService(dbContext);

            ParcelaUpdateValor dto = new ParcelaUpdateValor
            {
                ParcelaId = 1,
                ValorParcela = 999
            };


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.UpdateValorParcela(dto, UserID);


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //============================================================
            // Converte o IResult para o tipo concreto Created (que é o que Results.Created() )
            var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created>(resultado);

            // Agora você consegue acessar o StatusCode com segurança!
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }


        [Fact]
        public async Task UpdateStatusContaFixa()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ContasFixasService(dbContext);

            ContaFixaUpdateStatus dto = new ContaFixaUpdateStatus
            {
                Id_ContaFixa = 4,
                Status = false
            };


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.UpdateStatusContaFixa(dto, UserID);


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //============================================================
            // Converte o IResult para o tipo concreto Created (que é o que Results.Created() )
            var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created>(resultado);

            // Agora você consegue acessar o StatusCode com segurança!
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }

    }
}
