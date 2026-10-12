using Cloud_API.src.Model.Chunk;

namespace Cloud_API.src.Background
{
    public class UploadCleanupService : BackgroundService
    {
   
        private readonly UploadChunkTracker _chunkTracker;
        private readonly string RepositoryCache;
        private readonly ILogger<UploadCleanupService> _logger;

        // Configurações do Faxineiro
        private readonly TimeSpan CleanupIntervalMinutes = TimeSpan.FromMinutes(1); // Roda a cada 5 minutos
        private readonly TimeSpan FileRetentionMinutes = TimeSpan.FromMinutes(30); // Tolera 30 min parado

        public UploadCleanupService( IConfiguration configuration, UploadChunkTracker chunkTracker, ILogger<UploadCleanupService> logger)
        {
            _chunkTracker = chunkTracker;
            _logger = logger;
            RepositoryCache = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryCache").Value;

            FileRetentionMinutes = TimeSpan.FromMinutes(configuration.GetSection("BackgroundService:UploadCleanupService:FileRetentionMinutes").Get<int>());            
            CleanupIntervalMinutes = TimeSpan.FromMinutes(configuration.GetSection("BackgroundService:UploadCleanupService:CleanupIntervalMinutes").Get<int>());
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           // _logger.LogInformation("Serviço de limpeza de uploads ativado.");

            // Loop que roda enquanto a API estiver ligada
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                   // _logger.LogInformation("Iniciando varredura de uploads abandonados...");

                    // 1. Limpa a memória RAM e pega os tokens que foram descartados
                    var tokensRemovidos = _chunkTracker.LimparSessoesInativas(FileRetentionMinutes);

                   // _logger.LogInformation($"Varredura concluída. Tokens a serem removidos: {tokensRemovidos.Count}");

                    // 2. Limpa o Disco Rígido (Apaga as pastas temporárias dos tokens removidos)
                    foreach (var token in tokensRemovidos)
                    {
                        string tempFolderName = Path.Combine(RepositoryCache, token);
                     //   _logger.LogInformation($"Diretório Pasta Temporária: {tempFolderName}");

                        if (Directory.Exists(tempFolderName))
                        {
                            Directory.Delete(tempFolderName, true); // Deleta a pasta e todos os .part dentro
                            _logger.LogWarning($"Diretório {tempFolderName} removido.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro durante a execução da limpeza de uploads: {ex.Message}");
                }

                // Aguarda antes de rodar a próxima faxina
                await Task.Delay(CleanupIntervalMinutes, stoppingToken);
            }
        }   
    }
}
