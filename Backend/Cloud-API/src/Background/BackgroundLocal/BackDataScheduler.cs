namespace Cloud_API.src.Background.BackgroundLocal
{
    public class BackDataScheduler
    {
        private TimeSpan _intervaloExecucao;
        private Func<Task> _acaoExecutar;
        private CancellationTokenSource _cts;
        private Task? _execucaoTask;

        public BackDataScheduler(Func<Task> acaoExecutar, TimeSpan? intervaloExecucao = null)
        {
            _acaoExecutar = acaoExecutar;
            _intervaloExecucao = intervaloExecucao ?? TimeSpan.FromMinutes(5); // Padrão: 5 minutos
            _cts = new CancellationTokenSource();
        }

        public void Iniciar()
        {
            // Evita iniciar múltiplas tarefas se já estiver rodando
            if (_execucaoTask == null || _execucaoTask.IsCompleted)
            {
                _execucaoTask = Task.Run(async () =>
                {
                    // Loop infinito que executa a ação e depois espera o intervalo definido
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        try
                        {
                            // Executa a ação agendada
                            await _acaoExecutar();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro na execução agendada: {ex.Message}");
                        }
                        // Aguarda o intervalo definido antes de executar novamente, respeitando o token de cancelamento
                        await Task.Delay(_intervaloExecucao, _cts.Token);
                    }
                }, _cts.Token);
            }
        }

        public void Parar()
        {
            if (_execucaoTask != null && !_execucaoTask.IsCompleted)
            {
                _cts.Cancel();
                _execucaoTask.Wait(); // Espera a tarefa terminar de forma limpa
            }
        }

    }
}
