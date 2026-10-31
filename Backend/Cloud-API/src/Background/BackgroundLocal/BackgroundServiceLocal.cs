using Cloud_API.src.Repository.Dictionary;
using Microsoft.EntityFrameworkCore;

namespace Cloud_API.src.Background.BackgroundLocal
{
    public class BackgroundServiceLocal : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly DictionaryRepository _bancoDadosMemoria;
        private readonly ILogger<BackgroundServiceLocal> _logger;
        private BackDataScheduler? _scheduler;


        public BackgroundServiceLocal(IServiceScopeFactory scopeFactory, DictionaryRepository repo, ILogger<BackgroundServiceLocal> logger)
        {
            _scopeFactory = scopeFactory;
            _bancoDadosMemoria = repo;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Roda em segundo plano para não travar o Startup da API
            _ = Task.Run(async () => await CarregarRegistrosGradualmenteAsync(cancellationToken));
            return Task.CompletedTask;   
        }

        private async Task CarregarRegistrosGradualmenteAsync(CancellationToken cancellationToken)
        {
            int tamanhoLote = 100; // Carrega de 100 em 100 registros
            int pagina = 0;
            bool temMaisRegistros = true;

            _logger.LogInformation("[BackgroundServiceLocal] - Iniciando carga gradual dos registros...");

            while (temMaisRegistros && !cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Busca um lote do banco de dados
                    var lote = await dbContext.StorageRegistry
                        .Where(d => d.Ativo)
                        .Skip(pagina * tamanhoLote)
                        .Take(tamanhoLote)
                        .ToListAsync(cancellationToken);

                    if (lote.Count == 0)
                    {
                        temMaisRegistros = false;
                        break;
                    }

                    // Insere o lote no dicionário em memória
                    foreach (var item in lote)
                    {
                        _bancoDadosMemoria.BancoDadosMemoria[item.NameArchived] = item;
                    }

                    _logger.LogInformation($"[BackgroundServiceLocal] - Lote {pagina + 1} carregado. Total acumulado na RAM: {pagina * tamanhoLote + lote.Count}");

                    pagina++;

                    // Pequena pausa opcional para aliviar o banco de dados
                    await Task.Delay(100, cancellationToken);
                }
            }

            _logger.LogInformation("[BackgroundServiceLocal] - Carga gradual concluída com sucesso!");
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[BackgroundServiceLocal] - Parando o serviço...");
            _scheduler?.Parar();
            return Task.CompletedTask;
        }
    }
}
