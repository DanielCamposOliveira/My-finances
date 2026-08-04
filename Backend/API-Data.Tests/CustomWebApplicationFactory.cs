using API_Data.src.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace API_Data.Tests
{
    // CustomWebApplicationFactory é uma classe que herda de WebApplicationFactory<TProgram> e permite configurar o ambiente de teste da aplicação.
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        // Configura o ambiente de teste da aplicação, permitindo modificar os serviços registrados no contêiner de dependência.
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Configura os serviços de teste, permitindo substituir ou adicionar serviços específicos para o ambiente de teste.
            builder.ConfigureTestServices(services =>
            {
                //// 1. Remove qualquer registro previo do DbContextOptions
                //services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                //services.RemoveAll(typeof(DbContextOptions));

                //// 2. Aponta para o banco real PostgreSQL de testes
                //var testConnectionString = "Host=localhost;Port=5432;Database=MyFinances;Username=postgres;Password=123456";

                //services.AddDbContext<AppDbContext>(options =>
                //{
                //    options.UseNpgsql(testConnectionString);
                //});
            });
        }
    }
}