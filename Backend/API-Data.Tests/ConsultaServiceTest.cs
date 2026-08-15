using API_Data.src.Data;
using API_Data.src.Model;
using API_Data.src.Repository;
using API_Data.src.Services;
using Xunit.Abstractions;

namespace API_Data.Tests
{
    public class ConsultaServiceTest : TestBase
    {
        string id = Guid.NewGuid().ToString();
        string UserID = "21c8c222-6811-467e-8c1b-18f941349411";

        public ConsultaServiceTest(ITestOutputHelper output) : base(output)
        {
            EscreverLinha("Registro: " + DateTime.Now);
        }

        /// <summary>
        /// Cria uma instância do serviço TagService com os repositórios necessários.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        protected ConsultaService _ConsultaService(AppDbContext dbContext)
        {
            var repo = new ConsultaRepository(dbContext);
            return new ConsultaService(repo);
        }

        [Fact]
        public async Task TestTotalReceber()
        {
            // Arrange
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ConsultaService(dbContext);

      
            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = service.TotalReceber(UserID).Result;

            //=============================================================
            // 3. ASSERT (Verificação do resultado)
            //=============================================================
            // Aqui você pode fazer asserções com base nos dados reais do seu banco de dados
            EscreverLinha($"Total a Receber: {resultado}");

            Assert.NotNull( resultado );
            Assert.True(resultado >= 0, "O total a receber deve ser maior ou igual a zero.");  
        }


        [Fact]
        public async Task TotalSaldo()
        {
            // Arrange
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ConsultaService(dbContext);

            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = service.TotalSaldo(UserID).Result;

            //=============================================================
            // 3. ASSERT (Verificação do resultado)
            //=============================================================
            // Aqui você pode fazer asserções com base nos dados reais do seu banco de dados
            EscreverLinha($"Total a Receber: {resultado}");

            Assert.NotNull(resultado);
            Assert.True(resultado >= 0, "O total a receber deve ser maior ou igual a zero.");
        }



        [Fact]
        public async Task TotalQuitadasDoMes()
        {
            // Arrange
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ConsultaService(dbContext);


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado =  service.TotalQuitadasDoMes(UserID).Result;

            //=============================================================
            // 3. ASSERT (Verificação do resultado)
            //=============================================================
            // Aqui você pode fazer asserções com base nos dados reais do seu banco de dados
            EscreverLinha($"Total Quitadas do Mês: {resultado}");

            Assert.NotNull(resultado);
            Assert.True(resultado >= 0, "O total a receber deve ser maior ou igual a zero.");
        }

        [Fact]
        public async Task TotalDividasMes()
        {
            // Arrange
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ConsultaService(dbContext);

            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = service.TotalDividasMes(UserID).Result;

            //=============================================================
            // 3. ASSERT (Verificação do resultado)
            //=============================================================
            // Aqui você pode fazer asserções com base nos dados reais do seu banco de dados
            EscreverLinha($"Total Dividas do Mês: {resultado}");

            Assert.NotNull(resultado);
            Assert.True(resultado >= 0, "O total a receber deve ser maior ou igual a zero.");
        }

        [Fact]
        public async Task TotalContasPendentes()
        {
            // Arrange
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();
            var service = _ConsultaService(dbContext);


            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = service.TotalContasPendentes(UserID).Result;

            //=============================================================
            // 3. ASSERT (Verificação do resultado)
            //=============================================================
            // Aqui você pode fazer asserções com base nos dados reais do seu banco de dados
            EscreverLinha($"Total Contas Pendentes: {resultado}");

            Assert.NotNull(resultado);
            Assert.True(resultado >= 0, "O total a receber deve ser maior ou igual a zero.");
        }





    }
}
