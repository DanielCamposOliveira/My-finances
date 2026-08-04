using API_Data.src.Repository;
using API_Data.src.Services;
using Xunit.Abstractions;

namespace API_Data.Tests
{
    public class ConsultaServiceTest : TestBase
    {
        public ConsultaServiceTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task TestTotalReceber()
        {
            // Arrange
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();

            // Instanciamos o Repositório REAL com o DbContext REAL
            var ConsultaRepositoryReal = new ConsultaRepository(dbContext);

            // Instanciamos o Serviço REAL passando o Repositório REAL (sem Moq!)
            var ConsultaServiceReal = new ConsultaService(ConsultaRepositoryReal);



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado =  ConsultaServiceReal.TotalReceber().Result;

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

            // Instanciamos o Repositório REAL com o DbContext REAL
            var ConsultaRepositoryReal = new ConsultaRepository(dbContext);

            // Instanciamos o Serviço REAL passando o Repositório REAL (sem Moq!)
            var ConsultaServiceReal = new ConsultaService(ConsultaRepositoryReal);



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = ConsultaServiceReal.TotalSaldo().Result;

            //=============================================================
            // 3. ASSERT (Verificação do resultado)
            //=============================================================
            // Aqui você pode fazer asserções com base nos dados reais do seu banco de dados
            EscreverLinha($"Total a Receber: {resultado}");

            Assert.NotNull(resultado);
            Assert.True(resultado > 0, "O total a receber deve ser maior ou igual a zero.");
        }



        [Fact]
        public async Task TotalQuitadasDoMes()
        {
            // Arrange
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();

            // Instanciamos o Repositório REAL com o DbContext REAL
            var ConsultaRepositoryReal = new ConsultaRepository(dbContext);

            // Instanciamos o Serviço REAL passando o Repositório REAL (sem Moq!)
            var ConsultaServiceReal = new ConsultaService(ConsultaRepositoryReal);



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = ConsultaServiceReal.TotalQuitadasDoMes().Result;

            //=============================================================
            // 3. ASSERT (Verificação do resultado)
            //=============================================================
            // Aqui você pode fazer asserções com base nos dados reais do seu banco de dados
            EscreverLinha($"Total Quitadas do Mês: {resultado}");

            Assert.NotNull(resultado);
            Assert.True(resultado > 0, "O total a receber deve ser maior ou igual a zero.");
        }

        [Fact]
        public async Task TotalDividasMes()
        {
            // Arrange
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();

            // Instanciamos o Repositório REAL com o DbContext REAL
            var ConsultaRepositoryReal = new ConsultaRepository(dbContext);

            // Instanciamos o Serviço REAL passando o Repositório REAL (sem Moq!)
            var ConsultaServiceReal = new ConsultaService(ConsultaRepositoryReal);



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = ConsultaServiceReal.TotalDividasMes().Result;

            //=============================================================
            // 3. ASSERT (Verificação do resultado)
            //=============================================================
            // Aqui você pode fazer asserções com base nos dados reais do seu banco de dados
            EscreverLinha($"Total Dividas do Mês: {resultado}");

            Assert.NotNull(resultado);
            Assert.True(resultado > 0, "O total a receber deve ser maior ou igual a zero.");
        }

        [Fact]
        public async Task TotalContasPendentes()
        {
            // Arrange
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = DbContext();

            // Instanciamos o Repositório REAL com o DbContext REAL
            var ConsultaRepositoryReal = new ConsultaRepository(dbContext);

            // Instanciamos o Serviço REAL passando o Repositório REAL (sem Moq!)
            var ConsultaServiceReal = new ConsultaService(ConsultaRepositoryReal);



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = ConsultaServiceReal.TotalContasPendentes().Result;

            //=============================================================
            // 3. ASSERT (Verificação do resultado)
            //=============================================================
            // Aqui você pode fazer asserções com base nos dados reais do seu banco de dados
            EscreverLinha($"Total Contas Pendentes: {resultado}");

            Assert.NotNull(resultado);
            Assert.True(resultado > 0, "O total a receber deve ser maior ou igual a zero.");
        }





    }
}
