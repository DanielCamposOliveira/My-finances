using API_Data.src.Data;
using API_Data.src.DTOs.Lancamento;
using API_Data.src.Enum;
using API_Data.src.Repository;
using API_Data.src.Services;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;

namespace API_Data.Tests
{
    public class LancamentosServiceTest : TestBase
    {
        public LancamentosServiceTest(ITestOutputHelper output) : base(output)
        {
            EscreverLinha("Registro: " + DateTime.Now);
        }

        /// <summary>
        /// Cria uma instância do serviço TagService com os repositórios necessários.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected LancamentosService _lancamentosService(AppDbContext dbContext)
        {
            var repo = new LancamentosRepository(dbContext);
            return new LancamentosService(repo);
        }

        [Fact]
        public async Task ListarLancamentosAsync()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _lancamentosService(dbContext);


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.ListarLancamentosAsync("21c8c222-6811-467e-8c1b-18f941349411");

            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================
                  
            // converto a resposta
            var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<LancamentoResponse>>>(resultado);
            
            // verifico se o status é = OK
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            // verifico se não veio vazio
            var lancamentos = okResult.Value;
            Assert.NotEmpty(lancamentos);                       

        }

        [Fact]
        public async Task ListFaturaPendenteAsync()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _lancamentosService(dbContext);


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.ListFaturaPendenteAsync("21c8c222-6811-467e-8c1b-18f941349411");

            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================
            // converto a resposta
            var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<ParcelasResponse>>>(resultado);

            // verifico se o status é = OK
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            // verifico se não veio vazio
            var lancamentos = okResult.Value;
            Assert.NotEmpty(lancamentos);
        }

        [Fact]
        public async Task CriarLancamentoAsync()
         {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _lancamentosService(dbContext);

            var _lancamento = new Create
            {
                CategoriaId = 1,
                DataPrimeiroVencimento = DateTime.UtcNow,
                Descricao = $"LançamentoTest_{Guid.NewGuid():N}",
                QtdParcelas = 5,
                ValorTotal = 500,
                TagIds = []
            };



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.CriarLancamentoAsync(_lancamento, "21c8c222-6811-467e-8c1b-18f941349411");


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================

            var created = resultado as Microsoft.AspNetCore.Http.HttpResults.Created;

            Assert.NotNull(created);
            Assert.Equal(StatusCodes.Status201Created, created.StatusCode);

        }

        [Fact]
        public async Task UptateStatusLancamentoParcela()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _lancamentosService(dbContext);

            int ID_Parcela = 1;
            StatusParcela Status_Parcela = StatusParcela.Aberto;

            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await service.UptateStatusLancamentoParcela(ID_Parcela, Status_Parcela, "21c8c222-6811-467e-8c1b-18f941349411");


            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================
            var created = resultado as Microsoft.AspNetCore.Http.HttpResults.Created;

            Assert.NotNull(created);
            Assert.Equal(StatusCodes.Status201Created, created.StatusCode);

        }




    }
}
