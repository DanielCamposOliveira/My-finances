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
                CategoriaId = 1,
                Descricao = $"ContaFixaTest_{Guid.NewGuid():N}",
                DiaVencimento = 5,
                ValorBase = 155,
                TagIds = []
            };

            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.CriarContaFixaAsync(dados);


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
            var resultado = await service.GerarFaturasMesAsync();


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
            var resultado = await service.ListaTodasContasFixa();


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
            var resultado = await service.ListFaturaPendenteAsync();


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //============================================================
            // Converte o IResult para o tipo concreto Created (que é o que Results.Created() )
            var ValorResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<ParcelasResponse>>>(resultado);

            // Valida o StatusCode 200 OK
            Assert.Equal(StatusCodes.Status200OK, ValorResult.StatusCode);

            List<ParcelasResponse> ListaConta = ValorResult.Value;

            Assert.NotNull(ListaConta); // verifica se não é null
            Assert.NotEmpty(ListaConta); // verifica se não esta vazio
        }


        [Fact]
        public async Task UpdateStatusParcela()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ContasFixasService(dbContext);

            int Id_Parcela = 1;
            StatusParcela Status_Parcela = StatusParcela.Aberto;



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.UpdateStatusParcela(Id_Parcela, Status_Parcela);


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

            int Id_Parcela = 1;
            decimal Valor_Parcela = 999;



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.UpdateValorParcela(Id_Parcela, Valor_Parcela);


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

            int Id_ContaFixa = 1;
            bool Status_ContaFixa = false;



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.UpdateStatusContaFixa(Id_ContaFixa, Status_ContaFixa);


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
