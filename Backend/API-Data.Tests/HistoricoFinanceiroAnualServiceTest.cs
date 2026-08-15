using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Repository;
using API_Data.src.Services;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;

namespace API_Data.Tests
{
    public class HistoricoFinanceiroAnualServiceIntegrationTests : TestBase
    {
        string id = Guid.NewGuid().ToString();
        string UserID = "21c8c222-6811-467e-8c1b-18f941349411";

        public HistoricoFinanceiroAnualServiceIntegrationTests(ITestOutputHelper output) : base(output)
        {
            EscreverLinha("Registro: " + DateTime.Now);
        }

        /// <summary>
        /// Cria uma instância do serviço HistoricoFinanceiroAnualService com os repositórios necessários.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        #region Factories dos Serviços

        // Essa class é responsável por criar instâncias dos serviços
        protected ConsultaService CriarConsultaService(AppDbContext dbContext)
        {
            var repo = new ConsultaRepository(dbContext);
            return new ConsultaService(repo);
        }

        // Essa class é responsável por criar instâncias dos serviços
        protected HistoricoFinanceiroAnualService CriarHistoricoService(AppDbContext dbContext)
        {
            var repoHistorico = new HistoricoFinanceiroAnualRepository(dbContext);
            var serviceConsulta = CriarConsultaService(dbContext); // Reaproveita o factory do ConsultaService

            return new HistoricoFinanceiroAnualService(repoHistorico, serviceConsulta);
        }

        #endregion



        [Fact]
        public async Task ListaHistoricoAsync()
        {
            // 1. ARRANGE
            using var dbContext = DbContext();
            var service = CriarHistoricoService(dbContext);

            int anoAtual = DateTime.Today.Year;

            // 2. ACT
            IResult resultado = await service.ListaHistoricoAsync(anoAtual, UserID);

            // 3. ASSERT
            Assert.NotNull(resultado);

            // Converte o IResult para Ok<List<GraficoHistoricoResponse>>
            var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<GraficoHistoricoResponse>>>(resultado);

            // Valida o StatusCode 200 OK
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            // Extrai a lista de DTOs do retorno
            List<GraficoHistoricoResponse> listaGrafico = okResult.Value;
            
            //
            Assert.NotNull(listaGrafico);
            Assert.NotEmpty(listaGrafico);

            // Valida a estrutura dos dados do gráfico gerados
            var grafico = listaGrafico.First();
            Assert.Equal(2, grafico.ChartSeries.Count); // Saldo e Dívida
            Assert.Equal(12, grafico.ChartSeries[0].Data.Count); // 12 Meses
        }


        [Fact]
        public async Task UpdateHistoricoMesAsync()
        {
            // 1. ARRANGE
            using var dbContext = DbContext();
            var serviceHistorico = CriarHistoricoService(dbContext);


            var request = new HistoricoMesRequest
            {
                ano = DateTime.Today.Year,
                mes = DateTime.Today.Month,
                novoSaldo = 1000, // Novo saldo para o mês atual
                novaDivida = 500 // Nova dívida para o mês atual
            };


            // 2. ACT
            IResult resultado = await serviceHistorico.UpdateHistoricoMesAsync(request, UserID);


            // 3. ASSERT
             Assert.NotNull(resultado); // Verifica se o resultado não é nulo

                                        // Converte o IResult para o tipo concreto Created (que é o que Results.Created() gera)
            var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created>(resultado);

            // Agora você consegue acessar o StatusCode com segurança!
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }

        [Fact]
        public async Task GerarHistoricoMesAsync()
        {
            // 1. ARRANGE
            using var dbContext = DbContext();
            var serviceHistorico = CriarHistoricoService(dbContext);

            // 2. ACT
            IResult resultado = await serviceHistorico.GerarHistoricoMesAsync(UserID);

            // 3. ASSERT
            Assert.NotNull(resultado);

            // Checa se o resultado é Created (201) OU Conflict (409)
            bool ehResultadoValido = resultado is Microsoft.AspNetCore.Http.HttpResults.Created ||
                                     resultado is Microsoft.AspNetCore.Http.HttpResults.Conflict<string>;

            // Imprime para acompanhamento no console
            EscreverLinha($"Tipo de retorno recebido: {resultado.GetType().Name}");

            // O teste só passa se ehResultadoValido for true (se der Problem / 500 o teste falha aqui)
            Assert.True(ehResultadoValido, $"O serviço retornou um resultado inesperado: {resultado.GetType().Name}");

        }




    }
}