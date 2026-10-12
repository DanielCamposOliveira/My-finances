namespace Cloud_API.src.Background
{
    public class TaskBackground : BackgroundService
    {
        private readonly IFilaDeTrabalho _filaDeTrabalho;
        private readonly ILogger<TaskBackground> _logger;

        public TaskBackground(IFilaDeTrabalho fila, ILogger<TaskBackground> logger)
        {
            _filaDeTrabalho = fila;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[BackgroundService] - Iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Aguarda e pega a próxima tarefa
                    var tarefa = await _filaDeTrabalho.DesenfileirarTarefaAsync(stoppingToken);

                    _logger.LogInformation($"[BackgroundService] - Processando agora: {tarefa.Method.Name}");

                    // EXECUÇÃO: Roda a função com segurança
                    await tarefa(stoppingToken);

                    _logger.LogInformation($"[BackgroundService] - Finalizado com sucesso: {tarefa.Method.Name}");
                }
                catch (OperationCanceledException)
                {
                    // Captura o cancelamento normal quando o app está desligando
                    _logger.LogWarning("[BackgroundService] - Execução cancelada pelo encerramento do app.");
                }
                catch (Exception ex)
                {
                    // Captura qualquer erro na tarefa, loga e impede o BackgroundService de morrer
                    _logger.LogError($"[BackgroundService] - Erro crítico ao executar a tarefa da fila. Mensagem: {ex.Message}");
                }
            }
        }
    }
}
