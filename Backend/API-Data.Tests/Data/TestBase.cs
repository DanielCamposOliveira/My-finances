using API_Data.src.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace API_Data.Tests
{
    public abstract class TestBase
    {
        // Propriedade protegida disponível para todas as classes filhas
        protected readonly ITestOutputHelper Output;
        
        // 1. Log
        protected TestBase(ITestOutputHelper output)
        {
            Output = output;
        }

        // Método utilitário para escrever no console
        protected void EscreverLinha(string mensagem)
        {
            Output.WriteLine(mensagem);
        }

        protected AppDbContext DbContext()
        {
            var diretorioAtual = new DirectoryInfo(AppContext.BaseDirectory);

            while (diretorioAtual != null && !File.Exists(Path.Combine(diretorioAtual.FullName, "API-Data", "appsettings.json")))
            {
                diretorioAtual = diretorioAtual.Parent;
            }

            if (diretorioAtual == null)
            {
                throw new FileNotFoundException("Não foi possível localizar o arquivo appsettings.json do projeto API-Data.");
            }

            var caminhoAppSettings = Path.Combine(diretorioAtual.FullName, "API-Data", "appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile(caminhoAppSettings, optional: false)
                .Build();

            var connectionString = config.GetConnectionString("PostgreSQLConnection");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new AppDbContext(options);
        }
    }
}