using API_Data.src.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace API_Data.src.Background
{
    public class StartupQueueInitializer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWorkQueue _fila;
        private readonly TimeSpan _intervalo;


        public StartupQueueInitializer(IServiceProvider serviceProvider, IWorkQueue fila, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _fila = fila;

            // Lê as horas do appsettings.json (com valor 8 como fallback padrão)
            var horas = configuration.GetValue<double>("QueueSettings:IntervaloHoras", 8);
            _intervalo = TimeSpan.FromHours(horas);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 1. Executa imediatamente assim que a aplicação sobe
            await EnfileirarUsuariosAsync(stoppingToken);

            // 2. Configura o timer periódico para repetir a cada 8 horas
            using var timer = new PeriodicTimer(_intervalo);
            try
            {
                // O WaitForNextTickAsync aguarda 8 horas antes de cada repetição
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await EnfileirarUsuariosAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Encerramento gracioso do serviço quando a aplicação parar
            }
        }

        private async Task EnfileirarUsuariosAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Cria um escopo isolado para injetar o DbContext em cada execução de forma segura
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Obtém a lista dos IDs de utilizadores ativos
                var userIds = await db.Users
                    .AsNoTracking()
                    .Where(u => u.IsActive)
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken);

                // Enfileira cada ID para o QueueProcessor consumir
                foreach (var userId in userIds)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    await _fila.EnfileirarAsync(userId, cancellationToken);
                }
            }
            catch (Exception)
            {
                // Registe logs de erro se necessário, impedindo que a aplicação pare o timer
            }
        }
    }
}
